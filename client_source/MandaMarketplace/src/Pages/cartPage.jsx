import TopBar from "../Components/TopBar";
import HorizontalCard from "../Components/horizontalcard";
import logo from '../assets/old_main.jpeg';

export default function cartPage() {
    const item = {
        id: 1,
        title: "Sample Item",
        stock: 2,
        imageLink: logo,
        description: "This is a sample item description.",
        price: 19.99
    }
    return (
        <>
        <TopBar />
        <div className="container" style={{marginTop: "15px"}}>
            <HorizontalCard item={item} onClick={() => console.log("test")} onRemove={() => console.log("removed")}/>
        </div>
        </>
    )
}