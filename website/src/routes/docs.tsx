import { useState } from "react";
import { Sidebar, Menu, MenuItem, SubMenu } from "react-pro-sidebar";
import { Link, Outlet, useLocation } from "react-router-dom";
import { FaBook, FaQuestionCircle, FaCode, FaCube, FaHome } from "react-icons/fa";
import { FaGear } from "react-icons/fa6";
import "../styles/docs.css";

export default function Docs() {

  const mobile = window.matchMedia && window.matchMedia("(max-width: 600px)").matches;
  const lightMode = window.matchMedia && window.matchMedia("(prefers-color-scheme: light)").matches;

  const location = useLocation();

  const [collapsed, setCollapsed] = useState(mobile);
  const [eventAdded, setEventAdded] = useState(false);
  const [activeItem, setActiveItem] = useState(location.pathname);
  console.log(activeItem);

  const SIDEBAR_WIDTH = mobile ? "200px" : "250px";
  const SIDEBAR_COLLAPSED_WIDTH = "0px";
  const SIDEBAR_TRANSITION_DURATION = 600;

  // Ctrl+B sidebar
  if (!eventAdded) {

    setEventAdded(true);

    let internalCollapsed = collapsed;
    let internalCooldown = false;
    document.addEventListener("keydown", function (event) {
  
      if (internalCooldown) return;
  
      if (event.ctrlKey && event.key.toLowerCase() == "b") {

        internalCooldown = true;
        setTimeout(() => { internalCooldown = false; }, SIDEBAR_TRANSITION_DURATION);

        setCollapsed(!internalCollapsed);
        internalCollapsed = !internalCollapsed;
      }
    });
  }

  return (
    <>
      <Sidebar
        collapsed={collapsed}
        collapsedWidth={SIDEBAR_COLLAPSED_WIDTH}
        backgroundColor={lightMode ? "#f1f0f5" : "#1d1d1f"}
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
                color: disabled ? "#949396" : undefined,
                backgroundColor: active ? "#b8b3ff" : "#f1f0f5",
                borderRadius: level == 0 ? "7px" : undefined,
                "&:hover": {
                  backgroundColor: active? "#aca6ff" : "#dddce0",
                },
              };
            }

            else {
              return {
                color: disabled ? "#828282" : undefined,
                backgroundColor: active ? "#776ee0" : "#1d1d1f",
                borderRadius: level == 0 ? "7px" : undefined,
                "&:hover": {
                  backgroundColor: active? "#8e86f7" : "#2d2d30",
                },
              };
            }
          },
          subMenuContent() {
            return {
              backgroundCOlor: "#aaa444"
            }
          },
        }}
      >

      <MenuItem
        component={<Link to="/" />}
        icon={<FaHome />}>
          Home Page
      </MenuItem>

      <MenuItem
        component={<Link to="/docs/v2/getting-started" />}
        active={activeItem === "/docs/v2/getting-started" || activeItem === "/docs"}
        icon={<FaBook />}
        onClick={() => setActiveItem("/docs/v2/getting-started")}>
          Getting Started
      </MenuItem>

      <MenuItem
        component={<Link to="/docs/v2/how-does-it-work" />}
        active={activeItem === "/docs/v2/how-does-it-work"}
        icon={<FaQuestionCircle />}
        onClick={() => setActiveItem("/docs/v2/how-does-it-work")}>
          How does it work?
      </MenuItem>
      
      <SubMenu label="Development" icon={<FaCode />} defaultOpen={location.pathname.includes("/docs/v2/development/")}>
        <MenuItem
          component={<Link to="/docs/v2/development/authentication" />}
          active={activeItem === "/docs/v2/development/authentication"}
          icon={"🔒"}
          onClick={() => setActiveItem("/docs/v2/development/authentication")}>
            Authentication
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/development/security" />}
          active={activeItem === "/docs/v2/development/security"}
          icon={"⚔️"}
          onClick={() => setActiveItem("/docs/v2/development/security")}>
            Security
        </MenuItem>
        
        <MenuItem
          component={<Link to="/docs/v2/development/multiplayer" />}
          active={activeItem === "/docs/v2/development/multiplayer"}
          icon={"🤝"}
          onClick={() => setActiveItem("/docs/v2/development/multiplayer")}>
            Multiplayer
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/development/debugging" />}
          active={activeItem === "/docs/v2/development/debugging"}
          icon={"🐛"}
          onClick={() => setActiveItem("/docs/v2/development/debugging")}>
            Debugging
        </MenuItem>
      </SubMenu>

      <SubMenu label="API" icon={<FaCube />} defaultOpen={location.pathname.includes("/docs/v2/api/")}>
        <MenuItem
          component={<Link to="/docs/v2/api" />}
          active={activeItem === "/docs/v2/api"}
          icon={"🎨"}
          onClick={() => setActiveItem("/docs/v2/api")}>
            Static API
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/api/config" />}
          active={activeItem === "/docs/v2/api/config"}
          icon={"🔧"}
          onClick={() => setActiveItem("/docs/v2/api/config")}>
            Configuration
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/api/utils" />}
          active={activeItem === "/docs/v2/api/utils"}
          icon={"🎁"}
          onClick={() => setActiveItem("/docs/v2/api/utils")}>
            Utils
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/api/exceptions" />}
          active={activeItem === "/docs/v2/api/exceptions"}
          icon={"💥"}
          onClick={() => setActiveItem("/docs/v2/api/exceptions")}>
            Exceptions
        </MenuItem>
      </SubMenu>

      <SubMenu label="Internals" icon={<FaGear />} defaultOpen={location.pathname.includes("/docs/v2/internals/")}>
        <MenuItem
          component={<Link to="/docs/v2/internals/design" />}
          active={activeItem === "/docs/v2/internals/design"}
          icon={"📋"}
          onClick={() => setActiveItem("/docs/v2/internals/design")}>
            Design
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/internals/mock" />}
          active={activeItem === "/docs/v2/internals/mock"}
          icon={"🎭"}
          onClick={() => setActiveItem("/docs/v2/internals/mock")}>
            Mock
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/internals/webgl-template" />}
          active={activeItem === "/docs/v2/internals/webgl-template"}
          icon={"🏢"}
          onClick={() => setActiveItem("/docs/v2/internals/webgl-template")}>
            WebGL Template
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/internals/build-variables" />}
          active={activeItem === "/docs/v2/internals/build-variables"}
          icon={"🔗"}
          onClick={() => setActiveItem("/docs/v2/internals/build-variables")}>
            Build variables
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/internals/hirpc" />}
          active={activeItem === "/docs/v2/internals/hirpc"}
          icon={"👋"}
          onClick={() => setActiveItem("/docs/v2/internals/hirpc")}>
            hiRPC
        </MenuItem>
      </SubMenu>

      </Menu>
      </Sidebar>
      <Sidebar
        collapsed={collapsed}
        collapsedWidth={SIDEBAR_COLLAPSED_WIDTH}
        width={SIDEBAR_WIDTH}
        transitionDuration={SIDEBAR_TRANSITION_DURATION}
        style={{
          width:0,
          height:0,
        }}
      />
      <div id="outlet-container">
        <Outlet context={{onClick:() => setCollapsed(!collapsed), collapsed}}/>
      </div>
    </>
  );
}