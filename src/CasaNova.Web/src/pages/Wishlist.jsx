import { useAuth } from "../context/AuthContext";

export default function Wishlist() {

    const { wishlist } = useAuth();

    return (

        <div>

            <h1 className="text-3xl font-bold mb-5">

                Mis Favoritos

            </h1>

            {
                wishlist.length === 0
                    ?
                    <p>No tienes propiedades favoritas.</p>
                    :
                    wishlist.map(id => (

                        <div
                            key={id}
                            className="border rounded p-4 mb-3 shadow"
                        >

                            Propiedad #{id}

                        </div>

                    ))
            }

        </div>

    );

}