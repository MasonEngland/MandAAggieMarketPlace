import React from 'react';
import '../css/topbar.css';
import USU_logo from '../assets/USU_logo.jpg';
import Cookies from 'js-cookie';
import axios from 'axios';
import { useState } from 'react';
import { useEffect } from 'react';

export default function Topbar(props) {
    let selected = props.pageNumber;

    // initialize account section to be login and register
    const [ accountSection, setAccountSection ] = useState(
        <nav>
            <a href="/login" className={"nav-item" + (selected == 3 ? "selcted" : "")}>
                <b>Login</b>
            </a>
            <a href="/register" className={"nav-item" + (selected == 4 ? "selcted" : "")}>
                <b>Register</b>
            </a>
        </nav>
    );


    // check if user is logged in
    useEffect(() => {
        let token = Cookies.get('token');
        console.log(token);
        if (token) {
            axios.get("http://localhost:2501/Api/Accounts/GetAccount", {headers: {"authorization": 'Bearer ' + token}})
            .then((response) => {
                let email = response.data.account.email
                setAccountSection(
                    <nav>
                        <a href="/account" className={"nav-item" + (selected == 5 ? "selcted" : "")}><b>{email}</b></a>
                    </nav>
                );
            }).catch((error) => {
                console.log(error);
            });
        }
    }, []);
    


    // check if the current page is slected
    const navitems = [
        <a href="/" className={"nav-item " + (selected == 0 ? "selected" : "")} key="1">Home</a>,
        <a href="/browse" className={"nav-item " + (selected == 1 ? "selected" : "")} key="2">Browse</a>,
        <a href="/settings" className={'nav-item ' + (selected == 2 ? "selected": "")} key="3">Settings</a> 
    ];

    return (
        <header>
            <nav>
                <img src={USU_logo} alt="USU Logo" />
                <span>MandA Aggie Marketplace</span>
                {navitems}
                <input type="text" placeholder="Search"/>
                <button type="button"><span class="material-symbols-outlined">search</span></button>
            </nav>
            {accountSection}
        </header>
    );
}
