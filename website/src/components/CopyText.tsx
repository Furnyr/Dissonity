import "./CopyText.css";
import { COPY_BUTTON_PADDING, COPY_BUTTON_PADDING_MOBILE } from "../constants";
import { FaClipboardCheck, FaClipboardList } from "react-icons/fa";

function CopyText(props: { text: string, stateBool: boolean, stateFunction: (b: boolean) => void }) {

    const mobile = window.matchMedia && window.matchMedia("(max-width: 600px)").matches;

    const fontSize = mobile
        ? COPY_BUTTON_PADDING_MOBILE
        : COPY_BUTTON_PADDING;

    return (
        <>
            <code className="copy">{props.text}</code> <button style={{fontSize}} onClick={() => {
                navigator.clipboard.writeText(props.text);
          
                props.stateFunction(true);

                setTimeout(() => props.stateFunction(false), 2_000);
            }}>{props.stateBool ? <FaClipboardCheck /> : <FaClipboardList />}</button>
        </>
    );
}

export default CopyText