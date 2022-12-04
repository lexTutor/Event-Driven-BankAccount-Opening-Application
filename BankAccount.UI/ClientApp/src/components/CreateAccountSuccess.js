import React from 'react';
import { useNavigate } from "react-router-dom";

export function CreateAccountSuccess() {
    const navigate = useNavigate();

    const Home = () => {
        navigate("/");
    }

    return (
        <div>
            <p>Congratulations, your account creation is in process, Check your mail box soon </p>
            <button className="btn btn-primary" onClick={Home}>Go To Home</button>
        </div>
    );
}
