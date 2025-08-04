import Topbar from '../Components/topbar';
import styles from '../css/home.module.css';
import Card from '../Components/Itemcard';
import { useEffect, useState, useContext } from 'react';
import axios from 'axios';
import Cookies from 'js-cookie';
import serverUrl from '../util/serverurl';

export default function Home() {

    /**
     * this component is the home page and also works a featured page for the site
     * there are three sections which dsiplay different commonly searched items.
     * that means it makes 3 queries to get the data and thats what the useEffect hook is doing. 
     */
    const [cardData, setCardData] = useState([{
        imageLink: "https://www.usu.edu/advancement/named-spaces/images/oldmain.jpg",
        name: "Old Main",
        stock: 10,
        price: 10.00,
    }]);

    const [clothing, setClothing] = useState([]);
    const [gaming, setGaming] = useState([]);

    useEffect(() => {
        const token = Cookies.get('token');

        axios.get(`${serverUrl}/Api/Commerce/GetStock/1`)
        .then((response) => {
            setCardData(response.data.stock);
        })
        .catch((error) => {
            console.log(error);
        });

        axios.get(`${serverUrl}/Api/Commerce/Search/cloth`,
        {
            headers: {
                'Authorization': `Bearer ${token}`,
            }
        }
        ).then((response) => {
            setClothing(response.data.stock);
        })
        .catch((error) => {
            console.log(error);
        });

        axios.get(`${serverUrl}/Api/Commerce/Search/gaming`,
        {
            headers: {
                'Authorization': `Bearer ${token}`,
            }
        }
        ).then((response) => {
            setGaming(response.data.stock);
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
            </div>
            <h1>Clothing</h1>
            <div className={styles.cardContainer}>
                {clothing.map((data, index) => {
                    return <Card data={data} key={index}/>
                })}
            </div>
            <h1>Gaming</h1>
            <div className={styles.cardContainer}>
                {gaming.map((data, index) => {
                    return <Card data={data} key={index}/>
                })}
            </div>
        </div>
    )
}
