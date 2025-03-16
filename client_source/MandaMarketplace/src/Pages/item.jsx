import Topbar from '../Components/topbar';
import styles from '../css/item.module.css';
import img from '../assets/USU_logo.jpg';

export default function Item(props) {
    return (
        <div className={styles.item}>
            <Topbar pageNumber={12}/>
            <div className={styles.body}>
                <div className={styles.imageContainer}>
                    <img src={img} alt="item" className={styles.image}/>
                </div>
                <div className={styles.infoContainer}>
                    <h3>Old Main</h3>
                    <p>Old Main is the oldest building on the Utah State University campus. It was built in 1889 and is a historic landmark in the state of Utah. It is a beautiful building with a rich history.</p>
                    <span>Price: $10.00</span>
                    <span>Stock: 10</span>
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
