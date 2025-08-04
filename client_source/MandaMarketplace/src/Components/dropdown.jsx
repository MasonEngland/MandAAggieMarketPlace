import styles from '../css/dropdown.module.css';
import { useState, useRef } from 'react';

export default function Dropdown({ options, selectedOption, className, isSelector = true }) {

    const [isOpen, setIsOpen] = useState(false);
    const [currentOption, setCurrentOption] = useState(selectedOption);
    const [position, setPosition] = useState({ top: 0, left: 0, width: 0 });

    const triggerRef = useRef(null);


    // logic to calculate where dropdown goes
    const measureAndOpen = () => {
    if (triggerRef.current) {
        const rect = triggerRef.current.getBoundingClientRect();
        setPosition({
        top: rect.bottom,
        left: rect.left,
        width: rect.width,
        });
        setIsOpen(true);
    }
    };


    if (!isOpen) {
    return (
        <div
        ref={triggerRef}
        className={`${styles.closed} ${className}`}
        onMouseEnter={measureAndOpen}
        >
        {currentOption} <span className={"material-symbols-outlined " + styles.icon}>arrow_drop_down</span>
        </div>
    );
    }

    // here I use inline styling in order to set the poition of the dropdown menu directly under the selected element
    return (
    <>
        <div
        ref={triggerRef}
        className={`${styles.closed} ${className}`}
        onMouseEnter={measureAndOpen}
        onMouseLeave={() => setIsOpen(false)}
        >
        {currentOption} <span className={"material-symbols-outlined " + styles.icon}>arrow_drop_down</span>
        </div>

        <div
        className={`${styles.opened} ${className}`}
        style={{
            position: 'fixed',
            top: position.top,
            left: position.left,
            width: position.width,
            zIndex: 1000,
        }}
        onMouseEnter={measureAndOpen}
        onMouseLeave={() => setIsOpen(false)}
        >
        <ul className={styles.options}>
            {options.map((option, index) => (
            <li
                key={index}
                className={styles.option}
                onClick={() => {
                if (isSelector) setCurrentOption(option);
                setIsOpen(false);
                }}
            >
                {option}
            </li>
            ))}
        </ul>
        </div>
    </>
    );
}