
const DoxygenPage = () => {

  document.title = "Doxygen Generated Docs | Dissonity";

  return (
    <iframe
    src="/doxygen/index.html"
    style={{ width: "100%", height: "100vh", border: "none" }}
    title="HTML Viewer"
  ></iframe>
  );
}

export default DoxygenPage;