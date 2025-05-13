import axios from 'axios';
import Cookies from 'js-cookie';

// function that verifies user through server and reeturns the user object
export default function verifyAccount() {
    let cookie = Cookies.get("token");
    
    if (cookie == null || cookie === undefined || cookie === "") {
        return null;
    }
    const headers = {
        'Authorization': `Bearer ${cookie}`,
        'content-type': 'application/json'
    }
    axios.get("http://localhost:2501/Api/Accounts/GetAccount", {
        headers: headers
    }).then(res => {
        if (res.data.success === true) {
            return res.data.account;
        }
        else {
            return null;
        }
    }).catch(err => {
        console.log(err);
        return null;
    });
}