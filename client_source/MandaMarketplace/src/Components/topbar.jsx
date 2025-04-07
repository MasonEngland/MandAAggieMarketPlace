import React from 'react';
import styles from '../css/topbar.module.css';
import USU_logo from '../assets/USU_logo.jpg';
import Cookies from 'js-cookie';
import axios from 'axios';
import { useState } from 'react';
import { useEffect } from 'react';

export default function Topbar(props) {
    let selected = props.pageNumber;

    // initialize account section to be login and register
    const [ accountSection, setAccountSection ] = useState(
        <nav className={styles.nav}>
            <a href="/login" className={styles.a + " " + (selected == 3 ? styles.selected : "")}>
                <b className={styles.b}>Login</b>
            </a>
            <a href="/signup" className={styles.a + " " + (selected == 4 ? styles.selected : "")}>
                <b className={styles.b}>Register</b>
            </a>
        </nav>
    );
    
    // check if user is logged in
    useEffect(() => {
        let token = Cookies.get('token');

        if (!token || token == "") return;

        axios.get("http://localhost:2501/Api/Accounts/GetAccount", {headers: {"authorization": 'Bearer ' + token}})
        .then((response) => {
            let email = response.data.account.email
            setAccountSection(
                <nav>
                    <a href="/settings" className={styles.a + " " + (selected == 5 ? styles.selected : "")}><b className={styles.b}>{email}</b></a>
                </nav>
            );
        }).catch((error) => {
            console.log(error);
        });
    }, []);
    
    // check if the current page is slected
    const navitems = [
        <a href="/" className={styles.a + " " + (selected == 0 ? styles.selected : "")} key="1">Home</a>,
        <a href="/browse" className={styles.a + " "  + (selected == 1 ? styles.selected: "")} key="2">Browse</a>,
        <a href="/settings" className={styles.a + " " +  (selected == 2 ? styles.selected: "")} key="3">Settings</a> 
    ];

    return (
        <header className={styles.header}>
            <nav className={styles.nav}>
                <img id="logo" src={USU_logo} alt="USU Logo" className={styles.img} />
                <span className={styles.span} id ="title">MandA Aggie Marketplace</span>
                {navitems}
                <input type="text" placeholder="Search" className={styles.input}/>
                <button type="button" className={styles.button}><span id="material-symbols-outlined" className={styles.button + styles.materialSymbolsOutlined}>search</span></button>
            </nav>
            {accountSection}
        </header>
    );
}
