


import React, { useState } from "react";
import { contactAPI } from "./utilityFuncs.jsx";
export default { initialLoad, Login, Signup };



export async function initialLoad() {
    await contactAPI("load", "get", true)
        .then(function (value) {
            console.log(value);
            setData(value)
        });
}

export function Login({ onLogin, switchToSignup}) {
    const [emailAddress, setEmailAddress] = useState("");
    const [password, setPassword] = useState("");


    const handleSubmit = async () => {
        let loginData = await contactAPI("auth/login/", "post", true,
                { "EmailAddress": emailAddress, "Password": password });

        contactAPI("load/", "get", true)
            .then(function (value) {
                onLogin(value)
            });

    };

    return (
        <div>
            <input placeholder="email" onChange={e => setEmailAddress(e.target.value)} />
            <input type="password" onChange={e => setPassword(e.target.value)} />
            <button onClick={handleSubmit}>Login</button>

            <p onClick={switchToSignup} style={{ cursor: "pointer" }}>
                Don't have an account? Sign up
            </p>
        </div>
    );
}


export function Signup({ onSignup, switchToLogin }) {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [name, setName] = useState("");

    const handleSubmit = async () => {
            let registerData = await contactAPI("auth/register", "post", true,
                { "EmailAddress": email, "Password": password, "DisplayName": name }
            )
        const dashboardInfoData = await contactAPI("load", "get", true);
        onSignup(dashboardInfoData);
    };

    return (
        <div>
            <h2>Sign Up</h2>

            <input placeholder="name" onChange={e => setName(e.target.value)} />
            <input placeholder="email" onChange={e => setEmail(e.target.value)} />
            <input type="password" onChange={e => setPassword(e.target.value)} />

            <button onClick={handleSubmit}>Create Account</button>

            <p onClick={switchToLogin} style={{ cursor: "pointer" }}>
                Already have an account? Log in
            </p>
        </div>
    );
}