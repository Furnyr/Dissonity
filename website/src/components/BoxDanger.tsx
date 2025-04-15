import "./BoxDanger.css";
import "./NoticeBox.css";
import { MdDangerous } from "react-icons/md";

function BoxDanger(props: { title?: string, children: (JSX.Element | string)[] | JSX.Element | string }) {
    return (
        <div className="notice-box-container">
            <div className="box-danger">
                <div className="notice-box-icon">
                    <MdDangerous style={{fontSize:"25px"}}/>
                </div>
                <div className="notice-box-text">
                    <h3 className="notice-box-title">{props.title}</h3>
                    {props.children}
                </div>
            </div>
        </div>
    );
}

export default BoxDanger