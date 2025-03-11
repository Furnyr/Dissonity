import { useState } from "react";
import { Sidebar, Menu, MenuItem, SubMenu } from "react-pro-sidebar";
import { Link, Outlet, useLocation } from "react-router-dom";
import { FaBook, FaCode, FaCube, FaHome } from "react-icons/fa";
import { FaGear } from "react-icons/fa6";
import "../styles/docs.css";

export default function Docs() {

  //? Close sidebar
  let closeSidebar = false;

  const queryString = window.location.search;
  const urlParams = new URLSearchParams(queryString);
  const sidebarParam = urlParams.get("sidebar");

  if (sidebarParam == "false") {
    closeSidebar = true;
  }

  const mobile = window.matchMedia && window.matchMedia("(max-width: 600px)").matches;
  const lightMode = window.matchMedia && window.matchMedia("(prefers-color-scheme: light)").matches;

  const location = useLocation();

  const [collapsed, setCollapsed] = useState(mobile || closeSidebar);
  const [eventAdded, setEventAdded] = useState(false);

  const SIDEBAR_WIDTH = mobile ? "230px" : "250px";
  const SIDEBAR_COLLAPSED_WIDTH = "0px";
  const SIDEBAR_TRANSITION_DURATION = 300;

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

  // Close sidebar on mobile
  function closeSidebarMobile () {

    if (!mobile) return;

    setCollapsed(true);
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
          button: ({ level, active, disabled, isSubmenu }) => {
            if (lightMode) {
              return {
                color: disabled ? "#707070" : undefined,
                backgroundColor: isSubmenu
                  ? active ? "#cdcadb" : "#f1f0f5"
                  : active ? "#b8b3ff" : "#f1f0f5",
                borderRadius: level == 0 ? "7px" : undefined,
                "&:hover": {
                  backgroundColor: isSubmenu
                    ? active ? "#cdcadb" : "#dddce0"
                    : active ? "#aca6ff" : "#dddce0",
                },
              };
            }

            else {
              return {
                fontSize: mobile ? "13px" : "16px",
                color: disabled ? "#909090" : undefined,
                backgroundColor: isSubmenu
                  ? active ? "#383840" : "#1d1d1f"
                  : active ? "#776ee0" : "#1d1d1f",
                borderRadius: "7px",
                "&:hover": {
                  backgroundColor: isSubmenu
                  ? active ? "#383840" : "#2d2d30"
                  : active ? "#8e86f7" : "#2d2d30",
                },
              };
            }
          },
          subMenuContent() {
            return {
              backgroundColor: lightMode ? "#f1f0f5" : "#1d1d1f"
            };
          },
        }}
      >

      <MenuItem
        disabled={true}>
        Documentation
      </MenuItem>

      <MenuItem
        component={<Link to="/" />}
        icon={<FaHome />}>
          Home Page
      </MenuItem>

      <MenuItem
        component={<Link to="/guides" />}
        icon={<FaBook />}>
          Guides
      </MenuItem>

      <SubMenu
        label="API"
        icon={<FaCube />}
        defaultOpen={location.pathname.includes("/docs/v2/api/")}
        active={location.pathname.includes("/docs/v2/api") ? true : undefined}
      >
        <MenuItem
          component={<Link to="/docs/v2/api" />}
          active={location.pathname === "/docs/v2/api"}
          icon={"🎨"}
          onClick={closeSidebarMobile}>
            API Class
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/api/config" />}
          active={location.pathname === "/docs/v2/api/config"}
          icon={"⚙️"}
          onClick={closeSidebarMobile}>
            Configuration
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/api/utils" />}
          active={location.pathname === "/docs/v2/api/utils"}
          icon={"🎁"}
          onClick={closeSidebarMobile}>
            Utils
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/api/exceptions" />}
          active={location.pathname === "/docs/v2/api/exceptions"}
          icon={"💥"}
          onClick={closeSidebarMobile}>
            Exceptions
        </MenuItem>
      </SubMenu>
      
      <SubMenu
        label="Development"
        icon={<FaCode />}
        defaultOpen={location.pathname.includes("/docs/v2/development/")}
        active={location.pathname.includes("/docs/v2/development/") ? true : undefined}
      >
        <MenuItem
          component={<Link to="/docs/v2/development/performance" />}
          active={location.pathname === "/docs/v2/development/performance"}
          icon={"🚀"}
          onClick={closeSidebarMobile}>
            Performance
        </MenuItem>
        <MenuItem
          component={<Link to="/docs/v2/development/authentication" />}
          active={location.pathname === "/docs/v2/development/authentication"}
          icon={"🔒"}
          onClick={closeSidebarMobile}>
            Authentication
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/development/security" />}
          active={location.pathname === "/docs/v2/development/security"}
          icon={"⚔️"}
          onClick={closeSidebarMobile}>
            Security
        </MenuItem>
        
        <MenuItem
          component={<Link to="/docs/v2/development/multiplayer" />}
          active={location.pathname === "/docs/v2/development/multiplayer"}
          icon={"🤝"}
          onClick={closeSidebarMobile}>
            Multiplayer
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/development/debugging" />}
          active={location.pathname === "/docs/v2/development/debugging"}
          icon={"🐛"}
          onClick={closeSidebarMobile}>
            Debugging
        </MenuItem>
      </SubMenu>

      <SubMenu
        label="Internals"
        icon={<FaGear />}
        defaultOpen={location.pathname.includes("/docs/v2/internals/")}
        active={location.pathname.includes("/docs/v2/internals/") ? true : undefined}
      >
        <MenuItem
          component={<Link to="/docs/v2/internals/local-development" />}
          active={location.pathname === "/docs/v2/internals/local-development"}
          icon={"📼"}
          onClick={closeSidebarMobile}>
            Local development
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/internals/design" />}
          active={location.pathname === "/docs/v2/internals/design"}
          icon={"📋"}
          onClick={closeSidebarMobile}>
            Design
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/internals/mock" />}
          active={location.pathname === "/docs/v2/internals/mock"}
          icon={"🎭"}
          onClick={closeSidebarMobile}>
            Mock
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/internals/webgl-template" />}
          active={location.pathname === "/docs/v2/internals/webgl-template"}
          icon={"🏢"}
          onClick={closeSidebarMobile}>
            WebGL Template
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/internals/build-variables" />}
          active={location.pathname === "/docs/v2/internals/build-variables"}
          icon={"🔗"}
          onClick={closeSidebarMobile}>
            Build Variables
        </MenuItem>

        <MenuItem
          component={<Link to="/docs/v2/internals/hirpc" />}
          active={location.pathname === "/docs/v2/internals/hirpc"}
          icon={"👋"}
          onClick={closeSidebarMobile}>
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