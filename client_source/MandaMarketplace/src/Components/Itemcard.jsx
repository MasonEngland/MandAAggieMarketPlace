import styles from '../css/itemcard.module.css';
import { useNavigate } from 'react-router';

export default function Itemcard(props) {
    const navigate = useNavigate();
    let shortenedName = function() {

        // shorten longer names
        if (props.data.name.length > 40) {
            return props.data.name.substring(0, 40) + "...";
        }
        return props.data.name;
    }();
    
    const clickHandler = () => {
        navigate(`/item?item=${props.data.id}`);
    }

    return (
        <div className={styles.card}>
            <img src={props.data.imageLink} alt="item" />
            <h3 className={styles.h3}>{shortenedName}</h3>
            <span className={styles.span}>Price: ${props.data.price}</span>
            <span className={styles.span}>Stock: {props.data.stock}</span>
            <button onClick={() => clickHandler()}>Details</button>
        </div>
    )
}