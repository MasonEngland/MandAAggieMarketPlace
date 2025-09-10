import TopBar from "../Components/TopBar";
import HorizontalCard from "../Components/horizontalcard";

export default function cartPage() {
    const item = {
        id: 1,
        title: "Sample Item",
        stock: 2,
        imageLink: "https://via.placeholder.com/150",
        price: 19.99
    }
    return (
        <>
        <TopBar />
        <div className="container">
            <HorizontalCard item={item} onClick={() => console.log("test")} onRemove={() => console.log("removed")}/>
        </div>
        </>
    )
}