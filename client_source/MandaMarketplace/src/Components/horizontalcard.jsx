

export default function HorizontalCard({ item, onRemove, onClick }) {
    /**
     * item: { id, title, stock, imageLink, price }
     * onRemove: function to remove item from cart
     * onClick: function to navigate to item details page
     */
    return (
        <div onClick={() => onClick()}>
            <div>
                <img src={item.imageLink} alt="item" />
                <span>{item.title}</span>
                <span>Amount: {item.stock}</span>
                <span>Price: ${item.price}</span>
            </div>
            <div>
                <button onClick={() => onRemove()}>Remove</button>
            </div>
        </div>
    )
}