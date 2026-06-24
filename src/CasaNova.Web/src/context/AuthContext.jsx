import { createContext, useState } from "react";

export const AuthContext = createContext();

export function AuthProvider({ children }) {

    const [user, setUser] = useState(
        JSON.parse(localStorage.getItem("user"))
    );

    const login = (token, userInfo) => {

        localStorage.setItem("token", token);

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

    return (
        <AuthContext.Provider
            value={{
                user,
                login,
                logout
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}