import "./BoxWarn.css";
import "./NoticeBox.css";
import { IoIosWarning } from "react-icons/io";

function BoxWarn(props: { title?: string, children: (JSX.Element | string)[] | JSX.Element | string }) {
    return (
        <div className="notice-box-container">
            <div className="box-warn">
                <div className="notice-box-icon">
                    <IoIosWarning style={{fontSize:"25px"}}/>
                </div>
                <div className="notice-box-text">
                    <h3 className="notice-box-title">{props.title}</h3>
                    {props.children}
                </div>
            </div>
        </div>
    );
}

export default BoxWarn