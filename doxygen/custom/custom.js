
window.addEventListener("load", () => {

    const tabs = document.querySelectorAll("ul.children_ul > li");
    
    for (const tab of tabs) {

        const anchorElement = tab.querySelector(".item > span > a");
        const className = anchorElement.className;
        
        if (className == "namespaces.html") {
            tab.classList.add("navSeparator");
            break;
        }
    }
});