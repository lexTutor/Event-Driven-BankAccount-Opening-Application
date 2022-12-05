import { React, useState } from 'react';
import { useNavigate } from "react-router-dom";
import { postData } from './Utilities.js'

export const PotentialMember = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const handleClick = () => {
        setLoading(true);
        postData('https://localhost:44386/Orchestrator/initiateWorkflow',
            {
                workFlowId: 0,
                Metadata: JSON.stringify(
                {
                    WebsiteStartingUrl: "www.BankAccountUI/potentialMember",
                        ipAddress: "0:0:0",
                        initializationTime: new Date().toLocaleDateString()
                })
            })
            .then((data) =>
            {
                setLoading(false);
                navigate("/terms", {
                    state: {
                        sessionId: data.result
                    }
                });
            }
        );
    }

    return (
        <div>
            <h1>Welcome</h1>

            <p>Sign Up for an Account.</p>

            <button className="btn btn-primary" onClick={handleClick} >
                {loading ? "Loading..." : "Create Account"}</button>
        </div>
    );
}

