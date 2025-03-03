import { createRef, ReactNode, useEffect, useRef } from "react";
import { useLocation } from "react-router-dom";

function AutoScrollOnLoad({ children }: { children?: ReactNode }) {
    const containerRef = createRef<HTMLDivElement>();
    const location = useLocation();
    const lastHash = useRef("");
  
    useEffect(() => {

      // Scroll to top
      if (containerRef.current) {
        containerRef.current.scrollIntoView({ behavior: "instant" });
      }

      // Scroll to hash
      if (location.hash) {
        lastHash.current = location.hash.slice(1);
      }
      
      if (lastHash.current && document.getElementById(lastHash.current)) {
        document.getElementById(lastHash.current)?.scrollIntoView({ behavior: "instant" });
        lastHash.current = "";
      }
    }, [location.pathname]);
  
    return (
      <>
        <div ref={containerRef} style={{ overflowY:"visible" }} />
        {children}
      </>
    );
}

export default AutoScrollOnLoad