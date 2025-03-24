import Topbar from "../Components/topbar"
import styles from "../css/settings.module.css"
import {useEffect, useState} from 'react';
import Cookies from 'js-cookie';
import axios from 'axios';

export default function Settings() {
    const [account, setAccount] = useState({
        firstName: "",
        lastName: "",
        email: ""
    });

    useEffect(() => {
        let token = Cookies.get('token');
        console.log(token);

        if (token === null || token === undefined || token === "") {
            window.location.href = "/login";
            return;
        }
        
        axios.get("http://localhost:2501/Api/Accounts/GetAccount", {
            headers: {
                Authorization: `Bearer ${token}`
            }
        }).then(res => {
            console.log(res);
            if (res.data.success === true) {
                setAccount(res.data.account);
            }
            else {
                alert(res.data.message);
            }
        }).catch(err => {
            alert(err);
        });
        console.log(account);
    }, [])


    

    return (
        <div>
            <Topbar pageNumber={2}/>
            <div className={styles.container}>
                <h1>Account</h1>
                <label>
                    First Name: <br />
                    <input className={styles.input} type="text" value={account?.firstName}/>
                </label>
                <label>
                    Last Name: <br />
                    <input className={styles.input} type="text" value={account?.lastName}/>
                </label>
                <label>
                    Email: <br />
                    <input className={styles.input} type="text" value={account?.email}/>
                </label>
                <button className={styles.button}>Save Changes</button>
                <button className={styles.button}>Change Password</button>
                <button className={styles.button} onClick={() => location.href = "/addFunds"}>Add Funds</button>
            </div>
        </div>
    )
}   