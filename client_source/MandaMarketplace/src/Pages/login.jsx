import axios from 'axios'
import { useState } from 'react';
import Cookies from 'js-cookie';


export default function Login() {
    const [ email, setEmail ] = useState("");
    const [ password, setPassword ] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        const response = await axios.post("http://localhost:2501/Api/Auth/Login", {
            Email: email,
            Password: password,
            FirstName: "",
            LastName: "",
        });
        Cookies.set('token', response.data.token, { expires: 7});
        console.log(Cookies.get('token'));
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
    );
}
