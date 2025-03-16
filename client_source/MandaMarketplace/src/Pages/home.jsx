import Topbar from '../Components/topbar';
import styles from '../css/home.module.css';
import Card from '../Components/Itemcard';
import { useEffect, useState } from 'react';
import axios from 'axios';
import Cookies from 'js-cookie';
import { use } from 'react';

export default function Home() {

    let [cardData, setCardData] = useState([{
        imageLink: "https://www.usu.edu/advancement/named-spaces/images/oldmain.jpg",
        name: "Old Main",
        stock: 10,
        price: 10.00,
    }]);

    useEffect(() => {
        axios.get("http://localhost:2501/Api/Commerce/GetStock/1")
        .then((response) => {
            console.log(response.data);
            setCardData(response.data.stock);
        })
        .catch((error) => {
            console.log(error);
        });
    }, []);

    return (
        
        <div className={styles.home}>
            <Topbar pageNumber={0}/>
            <h1>Featured</h1>
            <div className={styles.cardContainer}>
                {cardData.map((data, index) => {
                    return <Card data={data} key={index}/>
                })}
                <Card data={cardData[0]}/>
                <Card data={cardData[0]}/>
                <Card data={cardData[0]}/>  
                <Card data={cardData[0]}/>
                <Card data={cardData[0]}/>
            </div>
        </div>
    )
}
