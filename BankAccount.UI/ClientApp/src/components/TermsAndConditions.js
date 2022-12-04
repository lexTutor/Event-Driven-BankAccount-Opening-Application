import React from 'react';
import { useNavigate, useLocation } from "react-router-dom";

export function TermsAndConditions(data) {
    const navigate = useNavigate();
    const location = useLocation();

    const handleAccept = () => {
        console.log(location.state.sessionId);
        navigate("/Create-account", {
            state: {
                sessionId: data.result
            }
        });
    }

    const handleDecline = () => {
        navigate("/");
    }

    return (
        <div>
            <h1>Hello, world!</h1>
            <p>Welcome to your new single-page application, built with:</p>

            <p>To help you get started, do agree to the terms and conditions </p>
            <ul>
                <li><strong>Quick Transaction completion</strong></li>
                <li><strong>Access to loans</strong></li>
                <li><strong>Reliable Security</strong></li>
            </ul>

            <div>
                <button className="btn btn-primary" style={{ marginRight: 10 }} onClick={handleAccept}>Accept</button>
                <button className="btn btn-danger" onClick={handleDecline}>Decline</button>
            </div>
        </div>
    );
}
