import styles from '../css/login.module.css';
import USU_logo from '../assets/USU_logo.jpg';
import {useState } from 'react';
import axios from 'axios';
import Cookies from 'js-cookie';
import {useNavigate} from 'react-router';
import serverUrl from '../util/serverurl';

export default function AddFunds() {
    /**
     * since the site doesnt work with real money, this is how you can easily add fake money to your account. 
     * users will purchase items with fake money that can be added an infinite amount of times. 
     * there is a chance a limit will need to be added but for now we can trust that users will not ruin the demo for other viewers.
     */

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

        if (isNaN(funds) || funds <= 0) {
            alert("Please enter a valid amount of funds to add");
            return;
        }

        const response = await axios.put(`${serverUrl}/Api/Accounts/Balance/${funds}`, {}, {headers: headers});
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