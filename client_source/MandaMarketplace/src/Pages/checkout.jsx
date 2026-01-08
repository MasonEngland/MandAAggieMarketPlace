import { useEffect } from "react";
import Topbar from "../Components/topbar";
import {loadStripe} from "@stripe/stripe-js";
import {useNavigate, useSearchParams} from "react-router";
import cookies from "js-cookie";
import serverUrl from "../util/serverurl";



export default function Checkout() {
    let [searchParams] = useSearchParams();
    const navigate = useNavigate();

    useEffect(() => {

        (async () => {
            const stripe = await loadStripe('pk_test_51SDSZSLewtJLDgghtnQxaBKuxzJ1YvVfa5N1cAXKoVz7poDffRYXrLJPDN1UQaRh5PotprjO8FZKqGZ7rGq1Eodo00MSSHS0ye');
            const sessionId = searchParams.get("session_id");

            if (!stripe || !sessionId) return;

            const token = cookies.get("token");
            if (!token) {
                navigate("/login");
            }

            const response = await fetch(`${serverUrl}/Api/Transactions/GetSecret/${sessionId}`, {headers: {"Authorization": `Bearer ${token}`}});
            const data = await response.json();
            console.log(data);

            if (!data.secret || !data.success) {
                alert("Failed to retrieve session details. Please try again.");
                return;
            }

            const checkout = await stripe.initEmbeddedCheckout({clientSecret: data.secret});
            checkout.mount('#checkout');
        })();
    })

    return (
        <>
            <Topbar pageNumber={-1} />
            <div id={"checkout"}></div>
        </>
    )
}