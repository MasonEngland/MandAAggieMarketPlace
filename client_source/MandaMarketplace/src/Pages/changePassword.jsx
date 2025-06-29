import styles from '../css/login.module.css';
import USU_logo from '../assets/USU_logo.jpg';
import {useState} from 'react';

export default function ChangePassword() {
    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const changePassword = async () => {
        if (newPassword !== confirmPassword) {
            alert("New password and confirm password do not match.");
            return;
        }

        try {
            const response = await fetch("/Api/Accounts/ChangePassword", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    oldPassword: oldPassword,
                    newPassword: newPassword,
                }),
            });

            if (response.ok || response.status === 200) {
                alert("Password changed successfully.");
                location.href = "/settings";
            } else {
                const errorData = await response.json();
                alert(`Error changing password: ${errorData.message}`);
            }
        } catch (error) {
            console.error("Error changing password:", error);
            alert("An unexpected error occurred. Please try again later.");
        }
    }

    return (
        <div className={styles.container}>
            <form className={styles.form}>
                <img className={styles.logo} src={USU_logo} alt="Logo" />
                <h3>MandA Aggie Marketplace</h3>
                <h2>Change Password</h2>
                <input className={styles.input} type="password" placeholder={'old password'} value={oldPassword} onChange={(e) => setOldPassword(e.target.value)}/>
                <input className={styles.input} type="password" placeholder={'new password'} value={newPassword} onChange={(e) => setNewPassword(e.target.value)}/>
                <input className={styles.input} type="password" placeholder={'confirm password'} value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)}/>
                <button className={styles.button} type="button" onClick={() => changePassword()}>Confirm</button>
                <button className={styles.button + " " + styles.signup} type="button" onClick={() => location.href = "/settings"}>Return</button>
            </form>
        </div>
    );
}