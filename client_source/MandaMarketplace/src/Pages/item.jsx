import Topbar from '../Components/topbar';
import styles from '../css/item.module.css';
import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import axios from 'axios';
import cookies from 'js-cookie';

export default function Item(props) {

    // define state variables
    const [item, setItem] = useState({});
    const [searchParams] = useSearchParams();
    const [quantity, setQuantity] = useState(0);
    const [address, setAddress] = useState('');


    // fetch item data from server
    useEffect(() => {
        const id = searchParams.get('item');

        if (!id || id === '') {
            location.href = '/home';
        }

        axios.get(`http://localhost:2501/Api/Commerce/GetItem/${id}`)
        .then(res => {
            if (res.data.success === true) {
                setItem(res.data.item);
            }
            else {
                alert(res.data.message);
            }
        })
        .catch(err => {
            alert(err);
            location.replace("/");
        });
    },[]);

    const purchase = () => {
        let token = "";
        try {
            token = cookies.get('token');
        } catch (err) {
            console.log(err);
            return;
        }
        if (token === null || token === undefined || token === "") {
            location.href = '/login';
            return;
        }

        const id = item.id;

        if (address === '' || quantity === 0) {
            alert('Please enter an address and quantity');
            return;
        }

        if (!token || token === '') {
            location.href = '/login';
        }
        else if (!id || id === '') {
            location.href = '/home';
        }

        item.stock = quantity;

        axios.post(`http://localhost:2501/Api/Commerce/Purchase/${id}`, {
            ...item
        }, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
        .then(res => {
            if (res.data.success === true) {
                console.log(res);
                alert(res.data.message);
                location.href = '/';
            }
            else {
                alert(res.data.message);
            }
        })
        .catch(err => {
            alert(err);
        });
    }


    return (
        <div className={styles.item}>
            <Topbar pageNumber={12}/>
            <div className={styles.body}>
                <div className={styles.imageContainer}>
                    <img src={item.imageLink} alt="item" className={styles.image}/>
                </div>
                <div className={styles.infoContainer}>
                    <h2>{item.name}</h2>
                    <span className={styles.price}>${item.price}</span>
                    <span>Here is just a placeholder description meant for testing how the info section looks when there is an item description. font size should be large to so that a lot of text can fill the section</span>
                    <span style={item.stock < 5 ? {color: 'red'} : {color: 'black'}}>
                        Stock: {item.stock}
                        </span>
                    <span>Recieve within 10 days</span>
                    <span className={styles.inputContainer}>
                        <input type="number" placeholder="Quantity" className={styles.input} onChange={(e) => setQuantity(e.target.value)}/>
                        <input type="text" placeholder="Address" className={styles.input} onChange={(e) => setAddress(e.target.value)}/>
                    </span>
                    <button className={styles.button} style={{cursor: 'pointer'}} onClick={() => purchase()}>Purchase</button>
                </div>
            </div>
        </div>
    );
}
