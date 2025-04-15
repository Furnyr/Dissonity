import "./HashLink.css";

function HashLink(props: { link: string }) {
    return (
        <a href={props.link}>
            <span translate="no" className="hash-link">#</span>
        </a>
    );
}

export default HashLink