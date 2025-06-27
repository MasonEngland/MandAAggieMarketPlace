import styles from '../css/login.module.css';
import USU_logo from '../assets/USU_logo.jpg';
import {useState, useEffect} from 'react';

export default function ChangePassword() {
    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    return (
        <div className={styles.container}>
            <form className={styles.form}>
                <img className={styles.logo} src={USU_logo} alt="Logo" />
                <h3>MandA Aggie Marketplace</h3>
                <h2>Change Password</h2>
                <input className={styles.input} type="password" placeholder={'old password'} value={oldPassword} onChange={(e) => setOldPassword(e.target.value)}/>
                <input className={styles.input} type="password" placeholder={'new password'} value={newPassword} onChange={(e) => setNewPassword(e.target.value)}/>
                <input className={styles.input} type="password" placeholder={'confirm password'} value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)}/>
                <button className={styles.button} type="button">Confirm</button>
                <button className={styles.button + " " + styles.signup} type="button" onClick={() => location.href = "/settings"}>Return</button>
            </form>
        </div>
    );
}