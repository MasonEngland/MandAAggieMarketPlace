import TopBar from "../Components/TopBar";
import HorizontalCard from "../Components/horizontalcard";
import { useEffect, useState } from "react";
import {useNavigate} from "react-router";
import serverUrl from "../util/serverurl";
import Cookies from "js-cookie";

export default function cartPage() {
    const [items, setItems] = useState([]);
    const navigate = useNavigate();


    const getCartItems = async () => {
        let token = Cookies.get("token");
        if (token == null || token === undefined || token === "") {
            navigate("/login");
        }

        const headers = {
            'Authorization': `Bearer ${token}`,
            'content-type': 'application/json'
        }
        const response = await fetch(`${serverUrl}/Api/Cart/GetCart`, { headers });
        const data = await response.json();
        console.log(data);
        setItems(data.cartItems);
    }

    const removeFromCart = async (itemId) => {
        let token = Cookies.get("token");
        if (token == null || token === undefined || token === "") {
            navigate("/login");
        }

        const options = {
            method : 'DELETE',
            headers: {
                'content-type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        }
        const uri = `${serverUrl}/Api/Cart/RemoveFromCart/${itemId}`;
        const response = await fetch(uri, options);
        if (response.status === 200) {
            getCartItems();
            return;
        }
        alert("failed to remove cart item: \n");
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
                items.map((item, index) => (<HorizontalCard key={index} item={item} onRemove={() => removeFromCart(item.id)}></HorizontalCard>))
            )}
        </div>
        </>
    )
}