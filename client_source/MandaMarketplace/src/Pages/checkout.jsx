import { useEffect } from "react";
import Topbar from "../Components/topbar";
import {loadStripe} from "@stripe/stripe-js";



export default function Checkout() {
    let [searchParams] = useSearchParams();
    //TODO: get session from URL and display

    useEffect(() => {

        (async () => {
            const stripe = await loadStripe('pk_test_51MroaKJH3j4Y2YzYkY1gX5nXo8v5y7Qz3F6hU6k3JH3j4Y2YkY1gX5nXo8v5y7Qz3F6hU6k3JH3j4Y2YkY1gX5nXo8v5y7Qz3F6hU6k3JH00abc12345');
            const sessionId = searchParams.get("session_id");
        })();

        




    })

    return (
        <>
            <Topbar pageNumber={-1} />
            <div id={"checkout"}></div>
        </>
    )
}