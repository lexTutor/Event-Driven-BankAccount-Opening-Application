import { React } from 'react';
import { useNavigate } from "react-router-dom";

export function PotentialMember() {
    const navigate = useNavigate();

    const handleClick = () => {
        navigate("/terms");
    }

    return (
        <div>
            <h1>Welcome</h1>

            <p>Sign Up for an Account.</p>

            <button className="btn btn-primary" onClick={handleClick} >Create Account</button>
        </div>
    );
}

