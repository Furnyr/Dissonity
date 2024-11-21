import { Outlet, Link } from 'react-router-dom';
import '../styles/sidebar.css';

export default function Docs() {
  return (
    <>
      <div id='sidebar'>
        <h1>WIP</h1>
        <nav>
          <ul>
            <li>
              <Link to={'/docs/1'}>Test1</Link>
            </li>
            <li>
              <Link to={'/docs/2'}>Test2</Link>
            </li>
          </ul>
        </nav>
      </div>
      <div id='page'>
        <Outlet />
      </div>
    </>
  );
}