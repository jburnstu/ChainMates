
import React, { useState } from "react";
import {  useNavigate, useLocation} from 'react-router-dom';
import { contactAPI } from "../utilityFuncs.jsx";


export default { Login, Signup }
export function Login({ onLogin}) {
    console.log("IN LOGIN")
    const navigate = useNavigate();
    const location = useLocation();
    const [emailAddress, setEmailAddress] = useState("");
    const [password, setPassword] = useState("");


    const handleSubmit = async () => {
        await contactAPI("auth/login/", "post", true,
            // At some point want to generalise this to take username too
            { "EmailAddress": emailAddress, "Password": password });

        let initialLoadData = await contactAPI("load/", "get", true);
        onLogin(initialLoadData);

        const from = location.state?.from?.pathname || "/home";
        navigate(from, { replace: true });
    };


    return (
        <div>
            <input placeholder="email" onChange={e => setEmailAddress(e.target.value)} />
            <input type="password" onChange={e => setPassword(e.target.value)} />
            <button onClick={handleSubmit}>Login</button>

            <p onClick={() => navigate("/signup/")} style={{ cursor: "pointer" }}>
                Don't have an account? Sign up
            </p>
        </div>
    );
}


export function Signup({ onLogin }) {
    const navigate = useNavigate();
    const location = useLocation();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [name, setName] = useState("");

    const handleSubmit = async () => {
        await contactAPI("auth/register", "post", true,
            { "EmailAddress": email, "Password": password, "DisplayName": name }
        )
        const dashboardInfoData = await contactAPI("load", "get", true);
        onLogin(dashboardInfoData);

        const from = location.state?.from?.pathname || "/home";
        navigate(from+"/", { replace: true });
    };

    return (
        <div>
            <h2>Sign Up</h2>

            <input placeholder="name" onChange={e => setName(e.target.value)} />
            <input placeholder="email" onChange={e => setEmail(e.target.value)} />
            <input type="password" onChange={e => setPassword(e.target.value)} />

            <button onClick={handleSubmit}>Create Account</button>

            <p onClick={() => navigate("/login/")} style={{ cursor: "pointer" }}>
                Already have an account? Log in
            </p>
        </div>
    );
}