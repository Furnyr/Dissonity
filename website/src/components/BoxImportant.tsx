import "./BoxImportant.css";
import "./NoticeBox.css";
import { MdNotificationImportant } from "react-icons/md";

function BoxImportant(props: { title?: string, children: (JSX.Element | string)[] | JSX.Element | string }) {
    return (
        <div className="notice-box-container">
            <div className="box-imp">
                <div className="notice-box-icon">
                    <MdNotificationImportant style={{fontSize:"25px"}}/>
                </div>
                <div className="notice-box-text">
                    <h3 className="notice-box-title">{props.title}</h3>
                    {props.children}
                </div>
            </div>
        </div>
    );
}

export default BoxImportant