export default function Notifications() {

    const notifications = [

        {
            id: 1,
            message: "Reserva confirmada correctamente."
        },

        {
            id: 2,
            message: "Tu identidad fue aprobada."
        },

        {
            id: 3,
            message: "Recordatorio de check-in 14:00."
        }

    ];

    return (

        <div>

            <h1 className="text-3xl font-bold mb-5">

                Notificaciones

            </h1>

            {
                notifications.map(notification => (

                    <div
                        key={notification.id}
                        className="border rounded shadow p-4 mb-3"
                    >

                        {notification.message}

                    </div>

                ))
            }

        </div>

    );

}