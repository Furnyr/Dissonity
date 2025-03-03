import "./Footer.css";
import { Link } from "react-router-dom";
import { GITHUB_LINK } from "../constants";

function Footer() {

    return (
        <>
            <div className="footer-container">
                <hr />
                <nav>
                <div className="footer-links">
                    <div className="footer-link-column">
                        <p className="footer-link-header">Resources</p>
                        <Link to="/">Home Page</Link>
                        <br />
                        <Link to="/docs">Documentation</Link>
                        <br />
                        <Link to="/guides">Guides</Link>
                    </div>
                    <div className="footer-link-column">
                        <p className="footer-link-header">About</p>
                        <Link to="/about">Team</Link>
                        {/*<br />
                        <Link to="/blog">Blog</Link>*/
                        //todo
                        }
                    </div>
                    <div className="footer-link-column">
                        <p className="footer-link-header">External</p>
                        <a href={GITHUB_LINK} target="_blank">GitHub</a>
                    </div>
                </div>
                <div className="footer-copyright">
                    <div className="footer-copyright-text">
                        <i>&copy; 2024 Dissonity. This project is not affiliated, endorsed, or sponsored by Discord Inc. or Unity Technologies.</i>
                    </div>
                </div>
                </nav>
            </div>
        </>
    );
}

export default Footer