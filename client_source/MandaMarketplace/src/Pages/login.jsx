import axios from 'axios'
import { AxiosError } from 'axios';
import { useState } from 'react';
import Cookies from 'js-cookie';
import styles from '../css/login.module.css';
import USU_logo from '../assets/USU_logo.jpg';


export default function Login() {
    const [ email, setEmail ] = useState("");
    const [ password, setPassword ] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        try {
            const response = await axios.post("/Api/Auth/Login", {
                Email: email,
                Password: password,
                FirstName: "",
                LastName: "",
            });
            if (response.status === 200 && response.data.success) {
                console.log(response.data);
                Cookies.set('token', response.data.token, { expires: 7});
                location.href = "/";
            } 
            else {
                alert("Login failed. Please check your email and password.");
            }
        } catch (error) {
            if (error instanceof AxiosError) {
                alert("Login failed. Please check your email and password.");
            } else {
                console.error("An unexpected error occurred:", error);
                alert("An unexpected error occurred. Please try again later.");
            }
        }
        
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
