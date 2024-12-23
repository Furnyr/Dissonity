import { useState } from 'react';
import { Sidebar, Menu, MenuItem, SubMenu } from 'react-pro-sidebar';
import { Link, Outlet, useLocation } from 'react-router-dom';
import { FaBook, FaQuestionCircle, FaCode, FaCircle, FaCube, FaHome } from 'react-icons/fa';
import { FaGear } from "react-icons/fa6";
import '../styles/docs.css';

export default function Docs() {
  /*
  
  <div id='sidebar'>
        <nav>
          <ul>
            <li>
              <Link to={'/docs/getting-started'}>Getting started</Link>
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
  */

  const location = useLocation();
  const mobile = window.matchMedia && window.matchMedia('(max-width: 600px)').matches;
  const lightMode = window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches;

  const [collapsed, setCollapsed] = useState(mobile);
  const [activeItem, setActiveItem] = useState(location.pathname);
  console.log(activeItem);

  const SIDEBAR_WIDTH = mobile ? '200px' : '250px';
  const SIDEBAR_COLLAPSED_WIDTH = '0px';
  const SIDEBAR_TRANSITION_DURATION = 600;

  return (
    <>
    <Sidebar
      collapsed={collapsed}
      collapsedWidth={SIDEBAR_COLLAPSED_WIDTH}
      backgroundColor={lightMode ? '#f1f0f5' : '#1d1d1f'}
      width={SIDEBAR_WIDTH}
      transitionDuration={SIDEBAR_TRANSITION_DURATION}
      style={{
        position: "fixed",
        top: 0,
        bottom: 0,
        left: 0,
        height: "auto",
        border: "solid black 1px"
      }}
    >
      <Menu
        menuItemStyles={{
          button: ({ level, active, disabled }) => {
            if (lightMode) {
              return {
                color: disabled ? '#949396' : undefined,
                backgroundColor: active ? '#b8b3ff' : '#f1f0f5',
                borderRadius: level == 0 ? '7px' : undefined,
                '&:hover': {
                  backgroundColor: active? '#aca6ff' : '#dddce0',
                },
              };
            }

            else {
              return {
                color: disabled ? '#828282' : undefined,
                backgroundColor: active ? '#776ee0' : '#1d1d1f',
                borderRadius: level == 0 ? '7px' : undefined,
                '&:hover': {
                  backgroundColor: active? '#8e86f7' : '#2d2d30',
                },
              };
            }
          },
          subMenuContent(_) {
            return {
              backgroundCOlor: '#aaa444'
            }
          },
        }}
      >

      <MenuItem
        component={<Link to='/' />}
        icon={<FaHome />}>
          Home Page
      </MenuItem>

      <MenuItem
        component={<Link to='/docs/getting-started' />}
        active={activeItem === '/docs/getting-started' || activeItem === '/docs'}
        icon={<FaBook />}
        onClick={() => setActiveItem('/docs/getting-started')}>
          Getting Started
      </MenuItem>

      <MenuItem
        component={<Link to='/docs/how-does-it-work' />}
        active={activeItem === '/docs/how-does-it-work'}
        icon={<FaQuestionCircle />}
        onClick={() => setActiveItem('/docs/how-does-it-work')}>
          How does it work?
      </MenuItem>
      
      <SubMenu label='Development' icon={<FaCode />} defaultOpen={location.pathname.includes("/docs/dev/")}>
        <MenuItem
          component={<Link to='/docs/dev/getting-started2' />}
          active={activeItem === '/docs/dev/getting-started2'}
          icon={<FaCircle style={{ fontSize: "7px" }} />}
          onClick={() => setActiveItem('/docs/dev/getting-started2')}>
            Getting Started2
        </MenuItem>
      </SubMenu>

      <SubMenu label='API' icon={<FaCube />} defaultOpen={location.pathname.includes("/docs/api/")}>
        <MenuItem
          component={<Link to='/docs/dev/getting-started2' />}
          active={activeItem === '/docs/dev/getting-started2'}
          icon={<FaCircle style={{ fontSize: "7px" }} />}
          onClick={() => setActiveItem('/docs/dev/getting-started2')}>
            Getting Started2
        </MenuItem>
      </SubMenu>

      <SubMenu label='Internals' icon={<FaGear />} defaultOpen={location.pathname.includes("/docs/api/")}>
        <MenuItem
          component={<Link to='/docs/dev/getting-started2' />}
          active={activeItem === '/docs/dev/getting-started2'}
          icon={<FaCircle style={{ fontSize: "7px" }} />}
          onClick={() => setActiveItem('/docs/dev/getting-started2')}>
            Getting Started2
        </MenuItem>
      </SubMenu>

      </Menu>
    </Sidebar>
    <Sidebar
      collapsed={collapsed}
      collapsedWidth={SIDEBAR_COLLAPSED_WIDTH}
      backgroundColor={lightMode ? '#f1f0f5' : '#1d1d1f'}
      width={SIDEBAR_WIDTH}
      transitionDuration={SIDEBAR_TRANSITION_DURATION}
      style={{
        width:0,
        height:0,
      }}
    />
    <div id='outlet-container'>
      <Outlet context={{onClick:() => setCollapsed(!collapsed), collapsed}}/>
    </div>
    </>
  );
}