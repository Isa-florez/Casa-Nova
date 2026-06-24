import { useEffect, useState } from "react";
import client from "../api/client";

export default function LandlordDashboard() {

    const [dashboard, setDashboard] = useState({});
    const [properties, setProperties] = useState([]);

    const [newProperty, setNewProperty] = useState({

        title: "",
        description: "",
        city: "",
        pricePerNight: "",
        capacity: "",
        imageUrl: ""

    });

    useEffect(() => {

        loadDashboard();
        loadProperties();

    }, []);

    const loadDashboard = async () => {

        try {

            const response = await client.get(
                "/reports/dashboard"
            );

            setDashboard(response.data);

        }
        catch (error) {

            console.error(error);

        }

    };


    const loadProperties = async () => {

        try {

            const response = await client.get(
                "/properties/my"
            );

            setProperties(response.data);

        }
        catch (error) {

            console.error(error);

        }

    };


    const exportExcel = async () => {

        try {

            const response = await client.get(
                "/reports/export",
                {
                    responseType: "blob"
                }
            );

            const url =
                window.URL.createObjectURL(
                    new Blob([response.data])
                );

            const link =
                document.createElement("a");

            link.href = url;

            link.setAttribute(
                "download",
                "ReporteReservas.xlsx"
            );

            document.body.appendChild(link);

            link.click();

        }
        catch (error) {

            console.error(error);

        }

    };


    const handleChange = (e) => {

        setNewProperty({

            ...newProperty,

            [e.target.name]:
                e.target.value

        });

    };


    const createProperty = async (e) => {

        e.preventDefault();

        try {

            await client.post(
                "/properties",
                newProperty
            );

            alert(
                "Propiedad creada correctamente"
            );

            loadProperties();

            setNewProperty({

                title: "",
                description: "",
                city: "",
                pricePerNight: "",
                capacity: "",
                imageUrl: ""

            });

        }
        catch (error) {

            console.error(error);

        }

    };



    return (

        <div>

            <h1 className="text-3xl font-bold mb-8">

                Dashboard Propietario

            </h1>


            <div className="grid md:grid-cols-4 gap-5 mb-10">

                <div className="bg-white shadow rounded p-6">

                    <h4>Ingresos Totales</h4>

                    <h2 className="text-3xl font-bold">

                        ${dashboard.totalRevenue}

                    </h2>

                </div>

                <div className="bg-white shadow rounded p-6">

                    <h4>Ocupación</h4>

                    <h2 className="text-3xl font-bold">

                        {dashboard.occupancyRate}%

                    </h2>

                </div>

                <div className="bg-white shadow rounded p-6">

                    <h4>Rentabilidad</h4>

                    <h2 className="text-3xl font-bold">

                        {dashboard.profitability}%

                    </h2>

                </div>

                <div className="bg-white shadow rounded p-6">

                    <h4>Propiedades</h4>

                    <h2 className="text-3xl font-bold">

                        {dashboard.totalProperties}

                    </h2>

                </div>

            </div>



            <button
                className="bg-green-600 text-white px-5 py-3 rounded mb-8"
                onClick={exportExcel}
            >
                Exportar Excel
            </button>



            <div className="bg-white shadow rounded p-6 mb-10">

                <h2 className="text-2xl font-bold mb-5">

                    Mis Propiedades

                </h2>

                <table className="w-full">

                    <thead>

                        <tr className="border-b">

                            <th>Título</th>
                            <th>Ciudad</th>
                            <th>Precio</th>
                            <th>Capacidad</th>

                        </tr>

                    </thead>

                    <tbody>

                        {

                            properties.map(property => (

                                <tr
                                    key={property.id}
                                    className="border-b"
                                >

                                    <td>

                                        {property.title}

                                    </td>

                                    <td>

                                        {property.city}

                                    </td>

                                    <td>

                                        ${property.pricePerNight}

                                    </td>

                                    <td>

                                        {property.capacity}

                                    </td>

                                </tr>

                            ))

                        }

                    </tbody>

                </table>

            </div>



            <div className="bg-white shadow rounded p-6">

                <h2 className="text-2xl font-bold mb-6">

                    Publicar Nueva Propiedad

                </h2>

                <form
                    className="space-y-4"
                    onSubmit={createProperty}
                >

                    <input
                        className="border p-3 rounded w-full"
                        placeholder="Título"
                        name="title"
                        value={newProperty.title}
                        onChange={handleChange}
                    />

                    <textarea
                        className="border p-3 rounded w-full"
                        rows="4"
                        placeholder="Descripción"
                        name="description"
                        value={newProperty.description}
                        onChange={handleChange}
                    />

                    <input
                        className="border p-3 rounded w-full"
                        placeholder="Ciudad"
                        name="city"
                        value={newProperty.city}
                        onChange={handleChange}
                    />

                    <input
                        className="border p-3 rounded w-full"
                        placeholder="Precio por noche"
                        name="pricePerNight"
                        value={newProperty.pricePerNight}
                        onChange={handleChange}
                    />

                    <input
                        className="border p-3 rounded w-full"
                        placeholder="Capacidad"
                        name="capacity"
                        value={newProperty.capacity}
                        onChange={handleChange}
                    />

                    <input
                        className="border p-3 rounded w-full"
                        placeholder="URL Imagen"
                        name="imageUrl"
                        value={newProperty.imageUrl}
                        onChange={handleChange}
                    />

                    <button
                        className="bg-blue-600 text-white px-5 py-3 rounded"
                    >
                        Crear Propiedad
                    </button>

                </form>

            </div>

        </div>

    );

}