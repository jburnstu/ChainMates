


import React, { useState } from "react";
import { contactAPI } from "./utilityFuncs.jsx";
export default { Login, Signup };

/////   This document handles the logic for logging in / signing up to the app.         //////
//////   DISCLOSURE: I used AI to generate the login and signup functions here (I've    //////
///////  since edited and refactored them a bit) as at the time I dind't want to      ////////
/////    focus on authentification, and just wanted something that worked.         ///////////
//////   I've sinced made sur emy understanding has caught up with the implementation.   /////


export async function initialLoad(callback) {
    await contactAPI("load", "get", true)
        .then(function (value) {
            callback(value)
        });
}

export function Login({ onLogin, switchToSignup}) {
    const [emailAddress, setEmailAddress] = useState("");
    const [password, setPassword] = useState("");

    const handleSubmit = async () => {
        await contactAPI("auth/login/", "post", true,
        // At some point want to generalise this to take username too
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
            await contactAPI("auth/register", "post", true,
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