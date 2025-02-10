import axios from 'axios'
import { useState } from 'react';

export default function Login() {
    const [ email, setEmail ] = useState("")
    const [ password, setPassword ] = useState("")

    const handleSubmit = async (e) => {
        e.preventDefault()

        console.log(password);
        
        const response = await axios.post("http://localhost:2501/Api/Auth/Login", {
            Email: email,
            Password: password,
            FirstName: "",
            LastName: "",
        })
        console.log(response)
    }


    return (
        <div>
            <form>
                <h1>Login</h1>
                <input type="text" placeholder="Email" onChange={(e) => setEmail(e.target.value)}/>
                <input type="password" placeholder="Password" onChange={(e) => setPassword(e.target.value)}/>
                <button type="button" onClick={(e) => handleSubmit(e)}>Login</button>
            </form>
        </div>
    )
}
