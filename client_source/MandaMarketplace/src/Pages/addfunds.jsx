import styles from '../css/login.module.css';
import USU_logo from '../assets/USU_logo.jpg';
import {useState } from 'react';

export default function AddFunds() {

    return (
        <div className={styles.container}>
            <form className={styles.form}>
                <img className={styles.logo} src={USU_logo} alt="Logo" />
                <h3>MandA Aggie Marketplace</h3>
                <h2>Add Funds</h2>
                <input className={styles.input} type="number" placeholder="Amount" />
                <button className={styles.button} type="button">Add Funds</button>
                <button className={styles.button + " " + styles.signup} type="button" onClick={() => location.href = "/settings"}>Return</button>
            </form>
        </div>
    );
}