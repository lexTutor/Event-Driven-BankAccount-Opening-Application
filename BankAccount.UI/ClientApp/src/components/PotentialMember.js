import { React } from 'react';
import { useNavigate } from "react-router-dom";
import { postData } from './Utilities.js'

export function PotentialMember() {
    const navigate = useNavigate();

    const handleClick = () => {
        postData('https://localhost:44386/initiateWorkflow',
            {
                workFlowId: 0,
                Metadata: JSON.stringify({
                    WebsiteStartingUrl: "www.BankAccountUI/potentialMember",
                    ipAddress: "0:0:0"
                })
            })
            .then((data) =>
            {
                console.log(data);
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

            <button className="btn btn-primary" onClick={handleClick} >Create Account</button>
        </div>
    );
}

