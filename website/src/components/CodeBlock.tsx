import { useEffect } from "react";
import hljs from "highlight.js/lib/core";
import "./CodeBlock.css";

function CodeBlock (props: { children?: string, language: string, noPre?: boolean }) {


    useEffect(() => {
        const elements = document.querySelectorAll(".code-block");
        elements.forEach(el => {
            if ((el as HTMLElement).dataset.highlighted) return;
            hljs.highlightElement(el as HTMLElement);
        });
    }, [props.children]);

    if (props.noPre) {
        return (
            <div className="code-block-container">
                <div className="div-code-block">
                    <code className={`code-block ${props.language}`}>
                        {props.children}
                    </code>
                </div>
            </div>
        );
    }

    return (
        <div className="code-block-container">
            <pre className="pre-code-block">
                <code className={`code-block ${props.language}`}>
                    {props.children}
                </code>
            </pre>
        </div>
    );
};

export default CodeBlock;