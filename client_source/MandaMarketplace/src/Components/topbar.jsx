import styles from '../css/topbar.module.css';
import USU_logo from '../assets/USU_logo.jpg';
import Cookies from 'js-cookie';
import axios from 'axios';
import { useState } from 'react';
import { useEffect } from 'react';
import { Link, useNavigate } from 'react-router';

export default function Topbar(props) {
    let selected = props.pageNumber;
    const [ search, setSearch ] = useState("");
    const navigate = useNavigate();

    const [ accountSection, setAccountSection ] = useState(
        <nav className={styles.nav}>
            <Link to="/login" className={styles.a + " " + (selected == 3 ? styles.selected : "")}>
                <b className={styles.b}>Login</b>
            </Link>
            <Link to="/signup" className={styles.a + " " + (selected == 4 ? styles.selected : "")}>
                <b className={styles.b}>Register</b>
            </Link>
        </nav>
    );
    
    useEffect(() => {
        let token = Cookies.get('token');

        if (!token || token == "") return;

        axios.get("/Api/Accounts/GetAccount", {headers: {"authorization": 'Bearer ' + token}})
        .then((response) => {
            let email = response.data.account.email
            setAccountSection(
                <nav>
                    <Link to="/settings" className={styles.a + " " + (selected == 5 ? styles.selected : "")}><b className={styles.b}>{email}</b></Link>
                </nav>
            );
        }).catch((error) => {
            console.log(error);
        });
    }, []);
    
    const navitems = [
        <Link to="/" className={styles.a + " " + (selected == 0 ? styles.selected : "")} key="1">Home</Link>,
        <Link to="/browse" className={styles.a + " "  + (selected == 1 ? styles.selected: "")} key="2">Browse</Link>,
        <Link to="/settings" className={styles.a + " " +  (selected == 2 ? styles.selected: "")} key="3">Settings</Link> 
    ];

    return (
        <header className={styles.header}>
            <nav className={styles.nav}>
                <img id="logo" src={USU_logo} alt="USU Logo" className={styles.img} />
                <span className={styles.span} id ="title">MandA Aggie Marketplace</span>
                {navitems}
                <input type="text" placeholder="Search" className={styles.input} value={search} onChange={e => setSearch(e.target.value)}/>
                <button 
                    type="button" 
                    className={styles.button}
                    onClick={() =>{ navigate(`/browse?search=${search}`); navigate(0);}}
                >
                    <span 
                        id="material-symbols-outlined" 
                        className={styles.button + styles.materialSymbolsOutlined}
                    >
                        search
                    </span>
                </button>
            </nav>
            {accountSection}
        </header>
    );
}
