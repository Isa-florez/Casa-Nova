import { useEffect, useState } from "react";
import client from "../api/client";

export default function TenantDashboard() {

    const [bookings, setBookings] = useState([]);

    useEffect(() => {
        loadBookings();
    }, []);

    const loadBookings = async () => {

        try {

            const response = await client.get("/bookings/my");

            setBookings(response.data);

        }
        catch (error) {

            console.error(error);

        }

    };

    return (

        <div>

            <h1 className="text-3xl font-bold mb-8">

                Mis Reservas

            </h1>

            <div className="bg-white shadow rounded p-6">

                <table className="w-full">

                    <thead>

                        <tr className="border-b">

                            <th className="text-left py-3">
                                Propiedad
                            </th>

                            <th>
                                Inicio
                            </th>

                            <th>
                                Fin
                            </th>

                            <th>
                                Check-In
                            </th>

                            <th>
                                Check-Out
                            </th>

                            <th>
                                Estado
                            </th>

                        </tr>

                    </thead>

                    <tbody>

                        {

                            bookings.map(booking => (

                                <tr
                                    key={booking.id}
                                    className="border-b"
                                >

                                    <td className="py-4">

                                        {booking.propertyTitle}

                                    </td>

                                    <td>

                                        {booking.startDate}

                                    </td>

                                    <td>

                                        {booking.endDate}

                                    </td>

                                    <td>

                                        14:00

                                    </td>

                                    <td>

                                        12:00

                                    </td>

                                    <td>

                                        <span className="bg-green-100 text-green-700 px-3 py-1 rounded">

                                            {booking.status}

                                        </span>

                                    </td>

                                </tr>

                            ))

                        }

                    </tbody>

                </table>

            </div>

        </div>

    );

}