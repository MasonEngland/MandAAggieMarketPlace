import Topbar from "../Components/Topbar";
import {useSearchParams} from "react-router";
import {useEffect, useState} from "react";
import axios from "axios";
import serverUrl from "../util/serverurl";
import cookies from "js-cookie";


export default function Status(){
    const [status, setStatus] = useState(0);

    const [searchParams] = useSearchParams();

    useEffect(() => {
        const token = cookies.get("token");
        const sessionId = searchParams.get("session_id");
        const address = searchParams.get("address");
        (async () => {
            const res = await axios.post(`${serverUrl}/Api/Transactions/CheckoutStatus`, {
                SessionId: sessionId,
                Address: address
            }, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            
            if(res.data.success === true){
                setStatus(1);
            } else {
                alert("Payment Failed" + res.data.message);
                setStatus(2);
            }
        })();
    }, []);

    const RenderStatus = () => {
        if(status === 0){
            return <p>Please wait while we process your payment.</p>;
        } else if(status === 1){
            return <p>Your payment was successful! Thank you for your purchase.</p>;
        } else {
            return <p>There was an issue with your payment. Please try again.</p>;
        }
    }

    return (
        <div>
            <Topbar />
            <div style={{marginTop: '100px', textAlign: 'center'}}>
                <h1>Processing Payment...</h1>
                <RenderStatus/>
            </div>
        </div>
    )
}