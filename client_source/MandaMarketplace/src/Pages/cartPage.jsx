import TopBar from "../Components/TopBar";
import HorizontalCard from "../Components/horizontalcard";
import { useEffect, useState } from "react";
import serverUrl from "../util/serverurl";
import Cookies from "js-cookie";

export default function cartPage() {
    const [items, setItems] = useState([]);

    const getCartItems = async () => {
        let token = Cookies.get("token");
        if (token == null || token === undefined || token === "") {
            location.href = "/login";
        }

        const headers = {
            'Authorization': `Bearer ${token}`,
            'content-type': 'application/json'
        }
        const response = await fetch(`${serverUrl}/Api/Cart/GetCart`, { headers });
        const data = await response.json();
        setItems(data.cartItems);
    }

    useEffect(() => {
       getCartItems();


    }, []);

    return (
        <>
        <TopBar />
        <div className="container" style={{marginTop: "15px"}}>
            {items.length === 0 ? (
                <h2>Your cart is empty.</h2>
            ) : (
                items.map((item, index) => (<HorizontalCard key={index} item={item} onRemove={() => setItems(items.filter(p => p.id != item.id))}></HorizontalCard>))
            )}
        </div>
        </>
    )
}