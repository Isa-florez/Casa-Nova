import { useState } from "react";
import axios from "axios";

export default function KycVerification() {

    const [image, setImage] = useState(null);
    const [result, setResult] = useState(null);

    const handleSubmit = async () => {

        const formData = new FormData();

        formData.append("file", image);

        try {

            const response = await axios.post(
                "/api/kyc",
                formData
            );

            setResult(response.data);

        }
        catch (error) {

            console.error(error);

        }
    };

    return (
        <div className="container mt-5">

            <h2>Validación de Identidad</h2>

            <input
                type="file"
                className="form-control"
                onChange={(e) =>
                    setImage(e.target.files[0])
                }
            />

            <button
                className="btn btn-primary mt-3"
                onClick={handleSubmit}
            >
                Procesar Documento
            </button>

            {
                result && (

                    <div className="card mt-4">

                        <div className="card-body">

                            <p>
                                Nombre:
                                {result.firstName}
                            </p>

                            <p>
                                Apellido:
                                {result.lastName}
                            </p>

                            <p>
                                Documento:
                                {result.documentNumber}
                            </p>

                            <p>
                                Fecha nacimiento:
                                {result.birthDate}
                            </p>

                            <p>
                                Estado:
                                {result.status}
                            </p>

                        </div>

                    </div>
                )
            }

        </div>
    );
}