import styles from '../css/itemcard.module.css';

export default function Itemcard(props) {
    
    const clickHandler = () => {
        location.href = `/item?item=${props.data.id}`;
    }

    return (
        <div className={styles.card}>
            <img src={props.data.imageLink} alt="item" />
            <h3 className={styles.h3}>{props.data.name}</h3>
            <span className={styles.span}>Price: ${props.data.price}</span>
            <span className={styles.span}>Stock: {props.data.stock}</span>
            <button onClick={() => clickHandler()}>Details</button>
        </div>
    )
}