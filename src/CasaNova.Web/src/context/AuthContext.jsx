import { createContext, useContext, useState } from "react";

export const AuthContext = createContext();

export function AuthProvider({ children }) {

    const [user, setUser] = useState(
        JSON.parse(localStorage.getItem("user"))
    );

    const [wishlist, setWishlist] = useState([]);

    const [deferredAction, setDeferredAction] = useState(null);


    const login = (token, userInfo) => {

        localStorage.setItem(
            "token",
            token
        );

        localStorage.setItem(
            "user",
            JSON.stringify(userInfo)
        );

        setUser(userInfo);

    };


    const logout = () => {

        localStorage.removeItem("token");

        localStorage.removeItem("user");

        setUser(null);

    };


    const toggleWishlist = (id) => {

        setWishlist(prev =>

            prev.includes(id)
                ? prev.filter(i => i !== id)
                : [...prev, id]

        );

    };


    return (

        <AuthContext.Provider
            value={{

                user,

                login,

                logout,

                wishlist,

                toggleWishlist,

                deferredAction,

                setDeferredAction

            }}
        >

            {children}

        </AuthContext.Provider>

    );

}


export const useAuth = () => {

    return useContext(
        AuthContext
    );

};