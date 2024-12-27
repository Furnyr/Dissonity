import "./CopyText.css";
import { COPY_BUTTON_PADDING } from "../constants";
import { FaClipboardCheck, FaClipboardList } from "react-icons/fa";

function CopyText(props: { text: string, stateBool: boolean, stateFunction: (b: boolean) => void }) {
    return (
        <>
            <code className="copy">{props.text}</code> <button style={{fontSize:COPY_BUTTON_PADDING}} onClick={() => {
                navigator.clipboard.writeText(props.text);
          
                props.stateFunction(true);

                setTimeout(() => props.stateFunction(false), 2_000);
            }}>{props.stateBool ? <FaClipboardCheck /> : <FaClipboardList />}</button>
        </>
    );
}

export default CopyText