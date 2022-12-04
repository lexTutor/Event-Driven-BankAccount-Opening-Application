import { React, useState } from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import { postData } from './Utilities.js'

export const CreateAccount = () => {
    const navigate = useNavigate();
    const location = useLocation();

    const [email, setEmail] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [loading, setLoading] = useState(false);

    const handleOnEmailChange = event => {
        const value = event.target.value;
        setEmail(value)
    };

    const handleOnFirstNameChange = event => {
        const value = event.target.value;
        setFirstName(value)
    };

    const handleOnLastNameChange = event => {
        const value = event.target.value;
        setLastName(value)
    };

    const handleOnSubmit = () => {
        setLoading(true);
        postData('https://localhost:44386/initiateWorkflow',
            {
                workFlowId: 1,
                sessionId: location.state.sessionId,
                Metadata: JSON.stringify(
                    {
                        email: email,
                        firstName: firstName,
                        lastName: lastName
                    })
            })
            .catch(() => {
                setLoading(false);
                navigate("/");
            })
            .then(() => {
                setLoading(false);
                navigate("/success");
            }
        );
    };

    return (
        <div>
            <h1>Best Decision of your life</h1>

            <form style={{ marginBottom: 30 }}>
                <label htmlFor="email">Email</label><br />
                <input type="Text" name="email" id="email" onChange={handleOnEmailChange} /><br />

                <label htmlFor="firstName">FirstName</label><br />
                <input type="Text" name="firstName" id="firstName" onChange={handleOnFirstNameChange} /><br />

                <label htmlFor="lastName">LastName</label><br />
                <input type="Text" name="lastName" id="lastName" onChange={handleOnLastNameChange} /><br />

            </form>

            <button className="btn btn-primary" onClick={handleOnSubmit}>
                {loading ? "Sending..." : "Create Account"}
            </button>
        </div>
    );
}

