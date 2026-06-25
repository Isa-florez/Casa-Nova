import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import client from "../api/client";
import { useAuth } from "../context/AuthContext";

export default function Marketplace() {

    const { wishlist, toggleWishlist } = useAuth();

    const [properties, setProperties] = useState([]);

    const [filters, setFilters] = useState({
        city: "",
        startDate: "",
        endDate: ""
    });

    useEffect(() => {
        loadProperties();
    }, []);

    const loadProperties = async () => {

        try {

            const response = await client.get("/properties", {
                params: filters
            });

            setProperties(response.data);

        }
        catch (error) {

            console.error(error);

        }

    };

    const handleChange = (e) => {

        setFilters({
            ...filters,
            [e.target.name]: e.target.value
        });

    };

    return (

        <div>

            <h1 className="text-4xl font-bold mb-8">

                Encuentra tu próximo alojamiento

            </h1>

            <div className="bg-white shadow rounded p-5 mb-8">

                <div className="grid md:grid-cols-4 gap-4">

                    <input
                        className="border p-3 rounded"
                        placeholder="Ciudad"
                        name="city"
                        onChange={handleChange}
                    />

                    <input
                        type="date"
                        className="border p-3 rounded"
                        name="startDate"
                        onChange={handleChange}
                    />

                    <input
                        type="date"
                        className="border p-3 rounded"
                        name="endDate"
                        onChange={handleChange}
                    />

                    <button
                        className="bg-blue-600 text-white rounded"
                        onClick={loadProperties}
                    >
                        Buscar
                    </button>

                </div>

            </div>


            <div className="grid md:grid-cols-3 gap-6">

                {
                    properties.map(property => (

                        <div
                            key={property.id}
                            className="bg-white shadow rounded overflow-hidden"
                        >

                            <img
                                src={property.imageUrl}
                                className="h-64 w-full object-cover"
                            />

                            <div className="p-5">

                                <h2 className="text-xl font-bold">

                                    {property.title}

                                </h2>

                                <p className="text-gray-500">

                                    {property.city}

                                </p>

                                <p className="mt-3">

                                    ${property.pricePerNight} / noche

                                </p>

                                <div className="flex gap-3 mt-5">

                                    <Link
                                        to={`/properties/${property.id}`}
                                        className="bg-blue-600 text-white px-4 py-2 rounded"
                                    >
                                        Ver detalle
                                    </Link>

                                    <button
                                        className="bg-red-500 text-white px-4 py-2 rounded"
                                        onClick={() =>
                                            toggleWishlist(property.id)
                                        }
                                    >
                                        {
                                            wishlist.includes(property.id)
                                                ? "♥"
                                                : "♡"
                                        }
                                    </button>

                                </div>

                            </div>

                        </div>

                    ))
                }

            </div>

        </div>

    );

}