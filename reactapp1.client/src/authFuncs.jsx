


import React, { useState } from "react";
import { contactAPI } from "./utilityFuncs.jsx";
export default { Login, Signup };


export function Login({ onLogin, switchToSignup}) {
    const [emailAddress, setEmailAddress] = useState("");
    const [password, setPassword] = useState("");

    //console.log("Inside Login component"); 
    //console.log(JSON.stringify({ "EmailAddress": emailAddress, "Password": password }));
    const handleSubmit = async () => {
        //try {
        let loginData = await contactAPI("auth/login/", "post", true,
                { "EmailAddress": emailAddress, "Password": password });

        contactAPI("dashboardInfo/", "get", true)
            .then(function (value) {
                onLogin(value)
            });

    };

        //    await fetch("/chainmates/auth/login", {
        //    method: "POST",
        //    credentials: "include",
        //    headers: {
        //        "Content-Type": "application/json"
        //    },
        //    body: JSON.stringify({ "EmailAddress": emailAddress, "Password": password })
        //});

        //console.log("/chainmates/auth/login -> status", loginRes.status);
        //if (!loginRes.ok) {
        //    const txt = await loginRes.text();
        //    console.error("Login failed:", loginRes.status, txt);
        //    return;
        //}

        //const res = await fetch("/chainmates/dashboardInfo", {
        //    credentials: "include"
        //});

            //console.log("/chainmates/dashboardInfo -> status", res.status);
            //if (!res.ok) {
            //    const txt = await res.text();
            //    console.error("Fetching dashboard failed:", res.status, txt);
            //    return;
            //}

         //onLogin(dashboardInfoData);
        //}
        //catch (err) {
        //    console.error("Login flow error", err);
        //}

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
        //try {
            console.log("IN HANDLE SUBMIT OF SIGNUP");
            console.log(JSON.stringify({ "EmailAddress": email, "Password": password, "DisplayName": name }));

            let registerData = await contactAPI("auth/register", "post", true,
                { "EmailAddress": email, "Password": password, "DisplayName": name }
            )

            //const regRes = await fetch("/chainmates/auth/register", {
            //    method: "POST",
            //    credentials: "include",
            //    headers: {
            //        "Content-Type": "application/json"
            //    },
            //    body: JSON.stringify({ "EmailAddress": email, "Password": password, "DisplayName": name })
            //});

            //console.log("/chainmtes/auth/register -> status", regRes.status);
            //if (!regRes.ok) {
            //    const txt = await regRes.text();
            //    console.error("Register failed:", regRes.status, txt);
            //    return;
            //}

            const dashboardInfoData = await contactAPI("dashboardInfo","get",true)

            //    await fetch("/chainmates/dashboardInfo", {
            //    credentials: "include"
            //});

            //console.log("/chainmates/dashboardInfo -> status", res.status);
            //if (!res.ok) {
            //    const txt = await res.text();
            //    console.error("Fetching dashboard failed:", res.status, txt);
            //    return;
            //}

            //const data = await res.json();
            onSignup(dashboardInfoData);
        //}
        //catch (err) {
        //    console.error("Signup flow error", err);
        //}
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