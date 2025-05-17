import axios from 'axios';
import Cookies from 'js-cookie';

// function that verifies user through server and reeturns the user object
export default async function verifyAccount() {
    let cookie = Cookies.get("token");

    const def = {authenticated: false};
    
    if (cookie == null || cookie === undefined || cookie === "") {
        return def;
    }
    const headers = {
        'Authorization': `Bearer ${cookie}`,
        'content-type': 'application/json'
    }
    const res = await axios.get("http://localhost:2501/Api/Accounts/GetAccount", {
        headers: headers
    });
    if (res.data.success === true) {
        //console.log(res.data.account);
        return {...res.data.account, authenticated: true};
    }
    else {
        return def;
    }
}
