import { Link } from "react-router-dom";
import { FaArrowUpRightFromSquare } from "react-icons/fa6";
import "./Button.css";

function Button(props: { text: string, link?: string, goto?: string, disabled?: boolean }) {
    if (props.disabled) {
        return (
            <div className="button-box">
                <button disabled={props.disabled} className="button-disabled">{props.text}</button>
            </div>
        );
    }

    if (props.link) {
        return (
            <div className="button-box">
                <a href={props.link} target="_blank">
                    <button>{props.text} {<i><FaArrowUpRightFromSquare style={{ fontSize: "17px" }}/></i>}</button>
                </a>
            </div>
        );
    }

    if (props.goto) {
        return (
            <div className="button-box">
                <Link to={props.goto}>
                    <button>{props.text}</button>
                </Link>
            </div>
        );
    }

    return (
        <div className="button-box">
            <button>{props.text}</button>
        </div>
    );
}

export default Button