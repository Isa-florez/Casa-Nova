import { useState } from "react";
import { useNavigate } from "react-router-dom";
import client from "../api/client";

export default function Auth() {

    const navigate = useNavigate();

    const [isLogin, setIsLogin] = useState(true);

    const [form, setForm] = useState({
        email: "",
        password: "",
        firstName: "",
        lastName: ""
    });

    const handleChange = (e) => {

        setForm({
            ...form,
            [e.target.name]: e.target.value
        });

    };

    const handleSubmit = async (e) => {

        e.preventDefault();

        try {

            if (isLogin) {

                const response = await client.post(
                    "/auth/login",
                    {
                        email: form.email,
                        password: form.password
                    }
                );

                localStorage.setItem(
                    "token",
                    response.data.token
                );

                localStorage.setItem(
                    "user",
                    JSON.stringify(response.data.user)
                );

                if (response.data.user.role === "Landlord") {

                    navigate("/landlord-dashboard");

                }
                else {

                    navigate("/tenant-dashboard");

                }

            }
            else {

                await client.post(
                    "/auth/register",
                    {
                        firstName: form.firstName,
                        lastName: form.lastName,
                        email: form.email,
                        password: form.password
                    }
                );

                alert("Usuario registrado correctamente");

                setIsLogin(true);

            }

        }
        catch (error) {

            console.error(error);

            alert("Error al autenticarse");

        }

    };

    return (

        <div className="container mt-5">

            <div className="row justify-content-center">

                <div className="col-md-5">

                    <div className="card shadow">

                        <div className="card-body">

                            <h2 className="mb-4 text-center">

                                {
                                    isLogin
                                        ? "Iniciar Sesión"
                                        : "Crear Cuenta"
                                }

                            </h2>

                            <form onSubmit={handleSubmit}>

                                {
                                    !isLogin &&
                                    <>
                                        <div className="mb-3">

                                            <label>Nombres</label>

                                            <input
                                                className="form-control"
                                                name="firstName"
                                                onChange={handleChange}
                                            />

                                        </div>

                                        <div className="mb-3">

                                            <label>Apellidos</label>

                                            <input
                                                className="form-control"
                                                name="lastName"
                                                onChange={handleChange}
                                            />

                                        </div>
                                    </>
                                }

                                <div className="mb-3">

                                    <label>Email</label>

                                    <input
                                        className="form-control"
                                        name="email"
                                        onChange={handleChange}
                                    />

                                </div>

                                <div className="mb-3">

                                    <label>Contraseña</label>

                                    <input
                                        type="password"
                                        className="form-control"
                                        name="password"
                                        onChange={handleChange}
                                    />

                                </div>

                                <button
                                    className="btn btn-primary w-100"
                                >
                                    {
                                        isLogin
                                            ? "Ingresar"
                                            : "Registrarse"
                                    }
                                </button>

                            </form>

                            <hr />

                            <button
                                className="btn btn-outline-secondary w-100"
                                onClick={() =>
                                    setIsLogin(!isLogin)
                                }
                            >

                                {
                                    isLogin
                                        ? "Crear una cuenta"
                                        : "Ya tengo cuenta"
                                }

                            </button>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    );

}