import { React, useState} from 'react';
import { useNavigate } from "react-router-dom";

export function CreateAccount() {
    const navigate = useNavigate();

    const [email, setEmail] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');

    const handleClick = () => {
        navigate("/");
    }

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
        console.log(firstName);
        console.log(lastName);
        console.log(email);
    };
    return (
        <div>
            <h1>Best Decision of your life</h1>

            <form style={{ marginBottom: 30 }}>
                <label for="email">Email</label><br/>
                <input type="Text" name="email" id="email" onChange={handleOnEmailChange} /><br />

                <label for="firstName">FirstName</label><br />
                <input type="Text" name="firstName" id="firstName" onChange={handleOnFirstNameChange} /><br />

                <label for="lastName">LastName</label><br />
                <input type="Text" name="lastName" id="lastName" onChange={handleOnLastNameChange} /><br />

            </form>
            <button className="btn btn-primary" onClick={handleOnSubmit}>Create Account</button>
        </div>
    );
}

