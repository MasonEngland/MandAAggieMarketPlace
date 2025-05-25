import Topbar from "../Components/topbar"
import styles from "../css/browse.module.css"
import { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router';
import Card from "../Components/itemcard";
import axios from 'axios';

export default function Browse() {
    const [items, setItems] = useState([]);
    const [searchParams] = useSearchParams();
    const search = searchParams.get('search') || '';

    useEffect(() => {
        const url = search !== '' ? `http://localhost:2501/Api/Commerce/Search/${search}` : 'http://localhost:2501/Api/Commerce/GetStock/2';
        axios.get(url)
            .then(res => {
                if (res.data.success === true) {
                    setItems(res.data.stock);
                } else {
                    alert(res.data.message);
                }
            })
            .catch(err => {
                alert(err);
            });
    }, []);

    return (
        <div>
            <Topbar pageNumber={1}/>
            <div className={styles.container}>
                {items.length >= 1 ? items.map((item, index) => (
                    <Card key={index} data={item} />
                )) : <h2>No Results</h2>}
            </div>
        </div>
    )

}