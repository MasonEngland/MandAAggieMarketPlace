import { useEffect } from "react";
import Topbar from "../Components/topbar";
import {loadStripe} from "@stripe/stripe-js";
import {useNavigate, useSearchParams} from "react-router";
import cookies from "js-cookie";



export default function Checkout() {
    let [searchParams] = useSearchParams();
    const navigate = useNavigate();

    useEffect(() => {

        (async () => {
            const stripe = await loadStripe('pk_test_51MroaKJH3j4Y2YzYkY1gX5nXo8v5y7Qz3F6hU6k3JH3j4Y2YkY1gX5nXo8v5y7Qz3F6hU6k3JH3j4Y2YkY1gX5nXo8v5y7Qz3F6hU6k3JH00abc12345');
            const sessionId = searchParams.get("session_id");

            if (!stripe || !sessionId) return;

            const token = cookies.get("token");
            if (!token) {
                navigate("/login");
            }

            const response = await fetch(`/api/checkout/session?session_id=${sessionId}`, {headers: {"Authorization": `Bearer ${token}`}});
            const data = await response.json();

            if (!data.clientSecret || !data.success) {
                alert("Failed to retrieve session details. Please try again.");
                return;
            }

            const checkout = stripe.initEmbeddedCheckout({clientSecret: data.clientSecret});
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