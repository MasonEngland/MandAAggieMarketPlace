import { useState } from "react";
import axios from "axios";
import styles from "../css/login.module.css";
import Cookies from "js-cookie";
import logo from "../assets/USU_logo.jpg";
import serverUrl from "../util/serverurl";

export default function Signup() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await axios.post(`${serverUrl}/Api/Auth/Create`, {
                Email: email,
                Password: password,
                FirstName: firstName,
                LastName: lastName,
            });
            console.log(response.data);
            if (response.data.success) {
                Cookies.set("token", response.data.token);
                location.href = "/";
            } else {
                alert("Registration failed: " + response.data.message);
            }

        } catch (error) {
            console.error("Error during registration:", error);
        }
    };

    return (
        <div className={styles.container}>
            <form className={styles.form} onSubmit={handleSubmit}>
                <img className={styles.logo} src={logo} alt="Logo" />
                <span className={styles.span}>MandA Aggie Marketplace</span><br/>
                <span className={styles.span}>Sign Up</span><br />
                <input
                    className={styles.input}
                    type="text"
                    placeholder="First Name"
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                    required
                />
                <input
                    className={styles.input}
                    type="text"
                    placeholder="Last Name"
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                    required
                />
                <input
                    className={styles.input}
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />
                <input
                    className={styles.input}
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                <button className={styles.button} type="submit">Sign Up</button>
                <button className={styles.button + " " + styles.signup} type="button" onClick= {() => location.href = "/login"}>Login</button>
                <span>Didn't want to login or signup? <a href="/" className={styles.a}>Return</a></span>
            </form>
        </div>
    );
}
