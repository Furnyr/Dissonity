import { useEffect } from "react";
import hljs from "highlight.js/lib/core";
import "./CodeBlock.css";

const lightMode = window.matchMedia && window.matchMedia("(prefers-color-scheme: light)").matches;

lightMode
    ? await import("highlight.js/styles/atom-one-light.css")
    : await import("highlight.js/styles/atom-one-dark.css");

function CodeBlock (props: { children?: string, language: string }) {


    useEffect(() => {
        const elements = document.querySelectorAll(".code-block");
        elements.forEach(el => {
            hljs.highlightElement(el as HTMLElement);
        });
    }, [props.children]);

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