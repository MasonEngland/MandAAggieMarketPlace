import { useState, useContext, useEffect } from 'react';
import { useNavigate } from 'react-router';
import axios from 'axios';
import cookies from 'js-cookie';
import serverUrl from '../util/serverurl';
import AuthContext from '../context/authContext';
import Topbar from '../Components/Topbar';
import styles from '../css/order.module.css';

export default function Orders(){
    
    const [orders, setOrders] = useState([]);
    const user = useContext(AuthContext);
    const navigate = useNavigate();

    useEffect(() => {
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

        (async () => {
            const res = await axios.get(`${serverUrl}/Api/Commerce/GetOrders`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if(res.data.success === true){
                console.log(res.data);
                setOrders(res.data.orders);
            } else {
                alert("Failed to fetch orders: " + res.data.message);
            }
        })();
    }, []);

    return (
        <div>
            <Topbar />
            <div className={styles.ordersContainer}>
                <h1 className={styles.ordersTitle}>Your Orders</h1>
                {orders.length === 0 ? (
                    <p className={styles.noOrders}>You have no orders.</p>
                ) : (
                    <ul className={styles.ordersList}>
                        {orders.map(order => (
                            <li key={order.id} className={styles.orderItem}>
                                <span className={styles.orderId}>Order ID: {order.id}</span> - Name: <span className={styles.orderItemName}>{order.orderItem.name}</span> - Delivered to: <span className={styles.orderAddress}>{order.address}</span>
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </div>
    )
}