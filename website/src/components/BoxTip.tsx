import "./BoxTip.css";
import "./NoticeBox.css";
import { FaLightbulb } from "react-icons/fa";

function BoxTip(props: { title?: string, children: (JSX.Element | string)[] | JSX.Element | string }) {
    return (
        <div className="notice-box-container">
            <div className="box-tip">
                <div className="notice-box-icon">
                    <FaLightbulb style={{fontSize:"21px"}}/>
                </div>
                <div className="notice-box-text">
                    <h3 className="notice-box-title">{props.title}</h3>
                    {props.children}
                </div>
            </div>
        </div>
    );
}

export default BoxTip