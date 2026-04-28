
import React, { useState } from "react";
import { useNavigate} from 'react-router-dom';
import { contactAPI } from "../utilityFuncs.jsx";

export default { SettingsPage }

export  function SettingsPage(props) {
    const navigate = useNavigate();

    const [newDisplayName, setNewDisplayName] = useState("");
    const [newEmailAddress, setNewEmailAddress] = useState("");
    const [newPassword, setNewPassword] = useState("");


    const handleLogout = () => {
        props.handleLogout();
        navigate("/login");
    }

    async function changeDisplayName(e) {
        await contactAPI("authors/", "patch", true, {
            displayName: newDisplayName
        });
    }
    async function changeEmailAddress(e) {
        await contactAPI("authors/", "patch", true, {
            emailAddress: newEmailAddress
        });
        handleLogout();
    }
    async function changePassword(e) {
        await contactAPI("authors/", "patch", true, {
            password: newPassword
        });
        handleLogout();
    }

    return (
        <fieldset>
            <label>Change Display Name
                <input label="Change Display Name" type="input"
                    value={newDisplayName}
                    onChange={(e) => setNewDisplayName(e.target.value)}>
                </input>
                <button type="submit" onClick={changeDisplayName} >{"->"}</button>
            </label>
            <label>Change Email Address
                <input label="Change Email Address" type="input"
                    value={newEmailAddress}
                    onChange={(e) => setNewEmailAddress(e.target.value)}>
                </input>
                <button type="submit" onClick={changeEmailAddress} >{"->"}</button>
            </label>
            <label>Change Password
                <input label="Change Password" type="input"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}>
                </input>
                <button type="submit" onClick={changePassword} >{"->"}</button>
            </label>
            <button onClick={handleLogout}>LOGOUT</button>
        </fieldset>

    )
}