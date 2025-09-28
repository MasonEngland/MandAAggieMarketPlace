import styles from '../css/horizontalcard.module.css';

export default function HorizontalCard({ item, onRemove, onClick }) {
    /**
     * item: { id, title, stock, imageLink, price }
     * onRemove: function to remove item from cart
     * onClick: function to navigate to item details page
     */
    return (
        <div onClick={() => onClick()} className={styles.body}>
            <img src={item.imageLink} alt="item" className={styles.image} />
            <div className={styles.content}>
                <div className={styles.info}>
                    <div className={styles.name_descrpiption}>
                        <span className={styles.span}>{item.title}</span>
                        <span className={styles.span}>{item.description}</span>
                    </div>
                    <div className={styles.stock_price}>
                        <span className={styles.span}>Amount: {item.stock}</span>
                        <span className={styles.span}>Price: ${item.price}</span>
                    </div>
                </div>
                <div className={styles.actions}>
                    <button onClick={() => onRemove()} className={styles.button}>Remove</button>
                    <button className={styles.button}>Details</button>
                </div>
            </div>
        </div>
    )
}