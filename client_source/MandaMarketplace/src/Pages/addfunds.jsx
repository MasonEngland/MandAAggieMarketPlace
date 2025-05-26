import styles from '../css/login.module.css';
import USU_logo from '../assets/USU_logo.jpg';
import {useState } from 'react';
import axios from 'axios';
import Cookies from 'js-cookie';
import {useNavigate} from 'react-router';

export default function AddFunds() {

    const [funds, setFunds] = useState(0);
    const navigate = useNavigate();

    const addFunds = async () => {
        let cookie = Cookies.get("token");
        if (cookie == null || cookie === undefined || cookie === "") {
            navigate("/login");
            return;
        }
        const headers = {
            "Authorization": `Bearer ${cookie}`,
        }

        const response = await axios.put(`/Api/Accounts/Balance/${funds}`, {}, {headers: headers});
        if (response.data.success) {
            alert("Funds added successfully");
            location.href = "/settings"; // do a hfref naviagate so that account data refreshes
        } else {
            alert("Failed to add funds");
        }
    }


    return (
        <div className={styles.container}>
            <form className={styles.form}>
                <img className={styles.logo} src={USU_logo} alt="Logo" />
                <h3>MandA Aggie Marketplace</h3>
                <h2>Add Funds</h2>
                <input className={styles.input} type="number" value={!isNaN(funds) ? funds : 0} onChange={(e) => setFunds(parseInt(e.target.value))} />
                <button className={styles.button} type="button" onClick={() => addFunds()}>Add Funds</button>
                <button className={styles.button + " " + styles.signup} type="button" onClick={() => location.href = "/settings"}>Return</button>
            </form>
        </div>
    );
}