import { Link } from 'react-router-dom';
import './Button.css';

function Button(props: { text: string, link?: string, goto?: string, disabled?: boolean }) {
    if (props.link) {
        return (
            <div className='button-box'>
                <a href={props.link} target='_blank'>
                    <button>{props.text} {<i className='fa-solid fa-arrow-up-right-from-square'></i>}</button>
                </a>
            </div>
        );
    }

    if (props.goto) {
        return (
            <div className='button-box'>
                <Link to={props.goto}>
                    <button>{props.text}</button>
                </Link>
            </div>
        );
    }

    if (props.disabled) {
        return (
            <div className='button-box'>
                <button disabled={props.disabled} className='button-disabled'>{props.text}</button>
            </div>
        );
    }

    return (
        <div className='button-box'>
            <button>{props.text}</button>
        </div>
    );
}

export default Button