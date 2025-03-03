import { useState } from "react";
import { Sidebar, Menu, MenuItem } from "react-pro-sidebar";
import { Link, Outlet, useLocation } from "react-router-dom";
import { FaHome, FaPlug } from "react-icons/fa";
import "../styles/docs.css";

export default function Guides() {

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
          Guides
      </MenuItem>

      <MenuItem
        component={<Link to="/" />}
        icon={<FaHome />}>
          Home Page
      </MenuItem>

      <MenuItem
        component={<Link to="/docs" />}
        icon={<FaPlug />}>
          Documentation
      </MenuItem>

      <MenuItem
        component={<Link to="/guides/v2/getting-started" />}
        active={location.pathname === "/guides/v2/getting-started" || location.pathname === "/guides"}
        icon={"🚀"}
        onClick={closeSidebarMobile}>
          Getting Started
      </MenuItem>

      <MenuItem
        component={<Link to="/guides/v2/how-does-it-work" />}
        active={location.pathname === "/guides/v2/how-does-it-work"}
        icon={"❓"}
        onClick={closeSidebarMobile}>
          How does it work?
      </MenuItem>

      <MenuItem
        component={<Link to="/guides/v2/why-dissonity" />}
        active={location.pathname === "/guides/v2/why-dissonity"}
        icon={"🧩"}
        onClick={closeSidebarMobile}>
          Why Dissonity?
      </MenuItem>

      <MenuItem
        component={<Link to="/guides/v2/migration-v2" />}
        active={location.pathname === "/guides/v2/migration-v2"}
        icon={"🔮"}
        onClick={closeSidebarMobile}>
          Migrating to v2
      </MenuItem>

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