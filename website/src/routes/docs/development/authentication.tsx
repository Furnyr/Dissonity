import CodeBlock from "../../../components/CodeBlock";
import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  return (
    <div className="doc-page">
        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1 id="start">Authentication <HashLink link="/docs/v2/development/authentication#start"/></h1> 

        <p>
          Dissonity automatically begins the <a href="https://discord.com/developers/docs/activities/how-activities-work#activity-lifecycle" target="_blank">Authorization and Authentication</a> process when the activity is launched based on your configuration file. This is why you don't have to call commands manually to start this process.
        </p>

        <p>
          Authentication is necessary before using any command, so <code>Api.Initialize</code> won't resolve until it is completed.
        </p>

        <h2 id="server-requirements">Server requirements <HashLink link="/docs/v2/development/authentication#server-requirements" /></h2>

        <p>
          Dissonity will send an HTTPS POST request with a JSON payload to the server using the specified request path. It expects a JSON response containing an access token:
        </p>

        <br />

        <h3>JSON Request</h3>

        <CodeBlock language="javascript">{`{
  "code": "<authorization code>"
}`}</CodeBlock>

        <h3>Expected JSON Response</h3>

        <CodeBlock language="javascript">{`{
  "token": "<authentication token>"
}`}</CodeBlock>

        <h3>Example server code</h3>

        <CodeBlock language="javascript">{`app.post("/api/token", async (req, res) => {

  if (!req.body.code) return res.status(400);
  
  const response = await fetch("https://discord.com/api/oauth2/token", {
    method: "POST",
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
    },
    body: new URLSearchParams({
      client_id: process.env.PUBLIC_CLIENT_ID!,
      client_secret: process.env.CLIENT_SECRET!,
      grant_type: "authorization_code",
      code: req.body.code,
    })
  });

  const { access_token } = await response.json();

  res.send({ token: access_token });
});`}</CodeBlock>

        <h2>External links</h2>

        <ul>
          <li><a href="https://discord.com/developers/docs/activities/how-activities-work#sample-code-and-activity-lifecycle-diagram" target="_blank">Client reference</a></li>
          <li><a href="https://discord.com/developers/docs/activities/building-an-activity#step-5-authorizing-authenticating-users" target="_blank">Server reference</a></li>
        </ul>

        <Footer />
    </div>
  );
}

export default DocsPage