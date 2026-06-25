import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import client from "../api/client";
import { useAuth } from "../context/AuthContext";

export default function PropertyDetail() {

    const navigate = useNavigate();

    const { user, setDeferredAction } = useAuth();

    const { id } = useParams();

    const [property, setProperty] = useState(null);

    const [booking, setBooking] = useState({
        startDate: "",
        endDate: ""
    });

    useEffect(() => {

        loadProperty();

    }, []);

    const loadProperty = async () => {

        try {

            const response = await client.get(
                `/properties/${id}`
            );

            setProperty(response.data);

        }
        catch (error) {

            console.error(error);

        }

    };


    const handleBooking = async () => {

        if (!user) {

            setDeferredAction({
                redirectTo: `/properties/${id}`
            });

            navigate("/auth");

            return;

        }

        try {

            await client.post(
                "/bookings",
                {
                    propertyId: id,
                    startDate: booking.startDate,
                    endDate: booking.endDate
                }
            );

            alert("Reserva realizada correctamente");

            navigate("/dashboard/tenant");

        }
        catch (error) {

            console.error(error);

            alert("No fue posible crear la reserva");

        }

    };



    if (!property)
        return <div>Cargando...</div>;


    return (

        <div className="grid md:grid-cols-2 gap-10">

            <div>

                <img
                    src={property.imageUrl}
                    className="rounded shadow"
                />

            </div>


            <div>

                <h1 className="text-4xl font-bold">

                    {property.title}

                </h1>

                <p className="text-gray-500 mt-2">

                    {property.city}

                </p>

                <p className="mt-5">

                    {property.description}

                </p>

                <h2 className="text-2xl font-bold mt-6">

                    ${property.pricePerNight} / noche

                </h2>


                <div className="mt-8 space-y-4">

                    <input
                        type="date"
                        className="border p-3 rounded w-full"
                        onChange={(e) =>
                            setBooking({
                                ...booking,
                                startDate: e.target.value
                            })
                        }
                    />

                    <input
                        type="date"
                        className="border p-3 rounded w-full"
                        onChange={(e) =>
                            setBooking({
                                ...booking,
                                endDate: e.target.value
                            })
                        }
                    />

                    <button
                        className="bg-green-600 text-white px-5 py-3 rounded"
                        onClick={handleBooking}
                    >
                        Reservar
                    </button>

                </div>

            </div>

        </div>

    );

}