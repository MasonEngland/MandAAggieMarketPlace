import TopBar from "../Components/Topbar";
import HorizontalCard from "../Components/horizontalcard";
import { useContext, useEffect, useState } from "react";
import {useNavigate} from "react-router";
import serverUrl from "../util/serverurl";
import Cookies from "js-cookie";
import AuthContext from "../context/authContext";
import styles from "../css/item.module.css";
import cookies from "js-cookie";
import axios from "axios";


export default function cartPage() {
    const [items, setItems] = useState([]);
    const [cartItems, setCartItems] = useState([]);
    const navigate = useNavigate();
    const user = useContext(AuthContext);


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
        setItems(data.items);
        setCartItems(data.cartItems);
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

    const purchaseCartItems = () => {


        // check user authentication and token validity
        if (!user.authenticated) {
            navigate('/login', {replace: true});
            return;
        }

        const token = cookies.get('token');
        if (token === null || token === undefined || token === '') {
            navigate('/login', {replace: true});
            return;
        }

        else if (cartItems.length === 0) {
            location.href = '/';
        }

        const address = cartItems[0].address;
        const body = cartItems.map(cartItem => ({
            ...cartItem,
            itemId: cartItem.orderItemId,
            quantity: cartItem.stock
        }));

        // make request to database
        axios.post(`${serverUrl}/Api/Transactions/Checkout/${address}`, JSON.stringify(body), {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        })
        .then(res => {
            console.log(res);
            if (res.data.success === true) {
                location.href = '/checkout?session_id=' + res.data.sessionId + '&address=' + address;
            }
            else {
                alert(res.data.message);
            }
        })
        .catch(err => {
            alert(err);
        });
    }
    useEffect(() => {
       getCartItems();
    }, []);


    const StatusSection = items.length === 0 ? (
        <></>
    ) : (
        <div style={{
            width: '50%', 
            height: '70vh', 
            backgroundColor: 'white', 
            padding: 24, 
            boxShadow: '0px 0px 10px rgba(0, 0, 0, 0.1)', 
            display : 'flex', 
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'space-between',
        }}>
            <div style={{backgroundColor: "white"}}>
                <h2 style={{backgroundColor: 'white'}}>{"Estimated Price"}</h2>

                {items.map((item, index) => (
                    <div key={index} style={{display: 'flex', flexDirection: 'row', justifyContent: 'space-around', backgroundColor: 'white', padding: 12}}>
                        <span style={{backgroundColor: 'white'}}>{item.name}</span>
                        <span style={{backgroundColor: 'white'}}>${item.price * item.stock}</span>
                    </div>
                ))}
                <span style={{fontSize: 24, fontWeight: 'bold', backgroundColor: 'white'}}>${items.reduce((total, item) => total + item.price * item.stock, 0).toFixed(2)}</span>
            </div>
            
            <button className={styles.button} style={{cursor: 'pointer', marginTop: 12}} onClick={() => purchaseCartItems()}>Purchase All Items</button> 
        </div>
    )

    return (
        <>
            <TopBar />
            <div style={{display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'start', marginTop: 24}}>
                <div className="container" style={{marginTop: "15px"}}>
                    {items.length === 0 ? (
                        <h2>Your cart is empty.</h2>
                    ) : (
                        items.map(
                            (item, index) => (
                                <HorizontalCard 
                                    key={index} 
                                    item={item} 
                                    onRemove={() => removeFromCart(item.id)} 
                                    onButton={() => navigate(`/item?item=${item.id}`)}>
                                </HorizontalCard>
                            )
                        )
                    )}
                </div>
                {StatusSection}
            </div>
            
        </>
    )
}