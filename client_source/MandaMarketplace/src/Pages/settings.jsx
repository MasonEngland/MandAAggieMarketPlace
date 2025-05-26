import Topbar from "../Components/topbar"
import styles from "../css/settings.module.css"
import {useEffect, useState, useContext} from 'react';
import AuthContext from "../context/authContext";
import Cookies from 'js-cookie';

export default function Settings() {
    const user = useContext(AuthContext);

    const [account, setAccount] = useState({
        firstName: "",
        lastName: "",
        email: ""
    });

    useEffect(() => {
        setAccount({
            firstName: user.firstName,
            lastName: user.lastName,
            email: user.email,
            balance: user.balance
        });
    }, [])


    const updateAccount = async () => {
        let token = Cookies.get("token");
        if (token == null || token === undefined || token === "") {
            location.href = "/login";
        }

        const headers = {
            'Authorization': `Bearer ${token}`,
            'content-type': 'application/json'
        }

        const options = {
            method: 'PUT',
            headers,
            body: JSON.stringify(account),
        }

        const response = await fetch("/Api/Accounts/Update", options)
        const res = await response.json();
        
        if (res.token == undefined ) return;
        Cookies.set("token", res.token);
        alert("succesfully updated account");
        location.href = "/";
    }



    return (
        <div>
            <Topbar pageNumber={2}/>
            <div className={styles.container}>
                <h1>Account</h1>
                <label>
                    First Name: <br />
                    <input className={styles.input} type="text" value={account?.firstName} onChange={(e) => setAccount(prev => {return {...prev, firstName: e.target.value}})}/>
                </label>
                <label>
                    Last Name: <br />
                    <input className={styles.input} type="text" value={account?.lastName} onChange={(e) => setAccount(prev => {return {...prev, lastName: e.target.value}})}/>
                </label>
                <label>
                    Email: <br />
                    <input className={styles.input} type="text" value={account?.email} onChange={(e) => setAccount(prev => {return {...prev, email: e.target.value}})}/>
                </label>
                <h4>Balance: ${account?.balance}</h4>
                <button className={styles.button} onClick={() => updateAccount()}>Save Changes</button>
                <button className={styles.button}>Change Password</button>
                <button className={styles.button} onClick={() => location.href = "/addFunds"}>Add Funds</button>
            </div>
        </div>
    )
}   