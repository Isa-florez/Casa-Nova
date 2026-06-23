namespace CasaNova.Application.Interfaces;

public interface IKycService
{
    Task<KycExtractionResult> ExtractDocumentDataAsync(Stream imageStream, CancellationToken ct = default);
}

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
}

public interface IStorageService
{
    Task<string> UploadEncryptedAsync(Stream file, string fileName, CancellationToken ct = default);
    Task DeleteAsync(string path, CancellationToken ct = default);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public record KycExtractionResult(
    bool Success,
    string? FirstName,
    string? LastName,
    string? DocumentNumber,
    DateTime? DateOfBirth,
    string? ErrorMessage
);

public interface IJwtService
{
    string GenerateToken(CasaNova.Domain.Entities.User user);
}