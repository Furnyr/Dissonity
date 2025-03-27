import { useEffect } from "react";
import mermaid from "mermaid";
import "./Mermaid.css";

function Mermaid(props: { syntax: string }) {

    useEffect(() => {

        const lightMode = window.matchMedia && window.matchMedia("(prefers-color-scheme: light)").matches;

        mermaid.initialize({
            startOnLoad: true,
            theme: lightMode
                ? "neutral"
                : "dark"
        });
        mermaid.contentLoaded();
    }, []);

    return (
        <div className="mermaid-div">
            <div className="mermaid">
                {props.syntax}
            </div>
        </div>
    );
};

export default Mermaid