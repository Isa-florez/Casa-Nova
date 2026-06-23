using CasaNova.Domain.Enums;
using CasaNova.Domain.Exceptions;
using CasaNova.Domain.Interfaces;
using CasaNova.Application.Interfaces;
using MediatR;

namespace CasaNova.Application.UseCases.KYC;

public record SubmitKycCommand(Stream DocumentImageStream, string FileName) : IRequest;

public class SubmitKycHandler : IRequestHandler<SubmitKycCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IKycService _kycService;
    private readonly IStorageService _storageService;

    public SubmitKycHandler(IUnitOfWork uow, ICurrentUserService currentUser,
        IKycService kycService, IStorageService storageService)
    {
        _uow = uow;
        _currentUser = currentUser;
        _kycService = kycService;
        _storageService = storageService;
    }

    public async Task Handle(SubmitKycCommand request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new UnauthorizedException();

        var user = await _uow.Users.GetByIdAsync(_currentUser.UserId.Value, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), _currentUser.UserId.Value);

        if (user.KycStatus == KycStatus.Approved)
            throw new DomainException("Tu identidad ya fue validada.");

        var extraction = await _kycService.ExtractDocumentDataAsync(request.DocumentImageStream, ct);
        if (!extraction.Success)
            throw new DomainException($"No se pudo procesar el documento: {extraction.ErrorMessage}");

        var encryptedPath = await _storageService.UploadEncryptedAsync(
            request.DocumentImageStream, request.FileName, ct);

        user.UpdateKycInfo(extraction.DocumentNumber!, extraction.DateOfBirth!.Value, encryptedPath);
        user.ApproveKyc();

        var notification = Domain.Entities.Notification.Create(
            user.Id,
            NotificationType.KycApproved,
            "Identidad verificada",
            "Tu documento fue validado exitosamente. Ya puedes realizar reservas."
        );
        await _uow.Notifications.AddAsync(notification, ct);

        await _uow.SaveChangesAsync(ct);
    }
}