import axios from 'axios'
import { useState } from 'react';
import Cookies from 'js-cookie';
import styles from '../css/login.module.css';
import USU_logo from '../assets/USU_logo.jpg';


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
        location.href = "/";
    }


    return (
        <div className={styles.container}>
            <form className={styles.form}>
                <img className={styles.logo} src={USU_logo} alt="Logo" />
                <h3>MandA Aggie Marketplace</h3>
                <h2>Login</h2>
                <input className={styles.input} type="text" placeholder="Email" onChange={(e) => setEmail(e.target.value)}/>
                <input className={styles.input} type="password" placeholder="Password" onChange={(e) => setPassword(e.target.value)}/>
                <button className={styles.button} type="button" onClick={(e) => handleSubmit(e)}>Login</button>
                <button className={styles.button + " " + styles.signup} type="button" onClick={() => window.location.href = "/signup"}>Sign Up</button>
                <span>didn't want to login or signup? <a href="/" className={styles.a}>return</a></span>
            </form>
            
        </div>
    );
}
