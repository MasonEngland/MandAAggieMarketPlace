import styles from '../css/topbar.module.css';
import USU_logo from '../assets/USU_logo.jpg';
import { useState, useEffect, useContext } from 'react';
import { Link, useNavigate } from 'react-router';
import Dropdown from './dropdown';
import AuthContext from '../context/authContext';

export default function Topbar(props) {
    let selected = props.pageNumber;
    const [ search, setSearch ] = useState("");
    const navigate = useNavigate();
    const user = useContext(AuthContext);


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


    // check if user is logged in and change account section accordingly
    useEffect(() => {
        if (user.authenticated) {
            let email = user.email;
            
            const ddOptions = [
                <Link to="/settings" className={styles.a}>Account</Link>,
                "View Order Status",
                "Go To Cart",
            ]
            setAccountSection(
                <nav>
                    <Dropdown options={ddOptions} selectedOption={email} isSelector={false}/>
                </nav>
            )
        }
    }, [user]);


    
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
                        className={"material-symbols-outlined"}
                    >
                        search
                    </span>
                </button>
            </nav>
            {accountSection}
        </header>
    );
}
