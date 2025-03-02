import "./BoxInfo.css";
import "./NoticeBox.css";
import { FaCircleInfo } from "react-icons/fa6";

function BoxInfo(props: { title?: string, children: (JSX.Element | string)[] | JSX.Element | string }) {
    return (
        <div className="notice-box-container">
            <div className="box-info">
                <div className="notice-box-icon">
                    <FaCircleInfo style={{fontSize:"21px"}}/>
                </div>
                <div className="notice-box-text">
                    <h3 className="notice-box-title">{props.title}</h3>
                    {props.children}
                </div>
            </div>
        </div>
    );
}

export default BoxInfo