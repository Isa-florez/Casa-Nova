import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Navbar() {

    const navigate = useNavigate();

    const { user, logout } = useAuth();

    return (

        <nav className="bg-slate-900 text-white shadow">

            <div className="max-w-7xl mx-auto px-6 py-4 flex justify-between items-center">

                <Link
                    to="/properties"
                    className="text-2xl font-bold"
                >
                    CasaNova
                </Link>

                <div className="flex items-center gap-5">

                    <Link
                        to="/properties"
                        className="hover:text-gray-300"
                    >
                        Explorar
                    </Link>

                    {
                        user &&

                        <Link
                            to="/wishlist"
                            className="hover:text-gray-300"
                        >
                            Favoritos
                        </Link>
                    }

                    {
                        user &&

                        <Link
                            to="/notifications"
                            className="hover:text-gray-300"
                        >
                            Notificaciones
                        </Link>
                    }

                    {

                        user?.role === "Guest" &&

                        <Link
                            to="/dashboard/tenant"
                            className="hover:text-gray-300"
                        >
                            Mis Reservas
                        </Link>

                    }

                    {

                        user?.role === "Owner" &&

                        <Link
                            to="/dashboard/landlord"
                            className="hover:text-gray-300"
                        >
                            Dashboard
                        </Link>

                    }

                    {

                        user
                            ?

                            <button
                                className="bg-red-500 px-4 py-2 rounded"
                                onClick={() =>
                                    logout(navigate)
                                }
                            >
                                Salir
                            </button>

                            :

                            <Link
                                to="/auth"
                                className="bg-blue-600 px-4 py-2 rounded"
                            >
                                Login
                            </Link>

                    }

                </div>

            </div>

        </nav>

    );

}