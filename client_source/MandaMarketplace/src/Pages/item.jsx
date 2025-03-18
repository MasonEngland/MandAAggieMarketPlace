import Topbar from '../Components/topbar';
import styles from '../css/item.module.css';
import img from '../assets/USU_logo.jpg';
import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import axios from 'axios';
import cookies from 'js-cookie';

export default function Item(props) {
    const [item, setItem] = useState({});
    const [searchParams] = useSearchParams();

    

    useEffect(() => {
        const id = searchParams.get('item');
        const token = cookies.get('token');

        if (!id || id === '') {
            location.href = '/home';
        }
        else if (!token || token === '') {
            location.href = '/login';
        }

        axios.get(`http://localhost:2501/Api/Commerce/GetItem/${id}`)
        .then(res => {
            if (res.data.success === true) {
                setItem(res.data.item);
                console.log(res.data.item);
            }
            else {
                alert(res.data.message);
            }
        })
        .catch(err => {
            alert(err);
            location.replace("/home");
        });
    },[]);




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
                    <span>
                            <input type="number" placeholder="Quantity" className={styles.input}/>
                    </span>
                    <button className={styles.button}>Purchase</button>
                </div>
            </div>
        </div>
    );
}
