import CollapseButton from "../../../components/CollapseButton";
import Footer from "../../../components/Footer";
import HashLink from "../../../components/HashLink";
import { useOutletContext } from "react-router-dom";
import { PageContext } from "../../../types";

import NoP2PLight from "../../../assets/diagrams/no_p2p_light.png";
import NoP2PDark from "../../../assets/diagrams/no_p2p_dark.png";
import ClientServerLight from "../../../assets/diagrams/client_server_light.png";
import ClientServerDark from "../../../assets/diagrams/client_server_dark.png";
import BoxInfo from "../../../components/BoxInfo";
import BoxTip from "../../../components/BoxTip";
import CodeBlock from "../../../components/CodeBlock";
import PageTitle from "../../../components/PageTitle";

function DocsPage () {

  const context = useOutletContext() as PageContext;

  const lightMode = window.matchMedia && window.matchMedia("(prefers-color-scheme: light)").matches;

  const diagram1 = lightMode
    ? NoP2PLight
    : NoP2PDark;

  const diagram2 = lightMode
    ? ClientServerLight
    : ClientServerDark;

  return (
    <div className="doc-page">
        <PageTitle title="Multiplayer | Dissonity"/>

        <CollapseButton onClick={context.onClick} collapsed={context.collapsed}/>

        <h1>Multiplayer <HashLink link="/?/docs/v2/development/multiplayer"/></h1> 

        <p>
          To make a multiplayer activity you need a way to connect different clients. In a normal activity, this can only be done through the <b>Discord Proxy</b> (<code>https://your-app-id.discordsays.com/.proxy</code>).
          This means you can't establish a <b>Peer to Peer</b> connection between players. Instead, you need to use a <b>Client-Server</b> design.
        </p>

        <p>
          This can be achieved by establishing two-way communication with your server, commonly via WebSocket, and synchronizing the other clients from there.
        </p>

        <h2 id="incorrect-connection-diagram">Incorrect connection diagram <HashLink link="/?/docs/v2/development/multiplayer#incorrect-connection-diagram" /></h2>

        <div className="overflow-div">
          <img src={diagram1} width={490} alt="Diagram of an incorrect connection structure"/>
        </div>

        <h2 id="correct-connection-diagram">Correct connection diagram <HashLink link="/?/docs/v2/development/multiplayer#correct-connection-diagram" /></h2>

        <div className="overflow-div">
          <img src={diagram2} width={800} alt="Diagram of a correct connection structure"/>
        </div>

        <h2 id="activity-instance-management">Activity Instance Management <HashLink link="/?/docs/v2/development/multiplayer#activity-instance-management" /></h2>

        <p>
          When a user joins an activity that another user started, it's said that they are in the same <b>application instance</b>. You can access the instance ID through <code>Api.InstanceId</code>. You should use this ID as a key to save shared game data, ensuring that two users who are in the same application instance hava access to the same data.
        </p>

        <BoxInfo title="Instance ID lifecycle">
          <p>
            Instance IDs are generated when a user launches an activity. The instance ID is discarded once all users have left the activity.
          </p>
        </BoxInfo>

        <h2 id="making-a-connection-inside-unity">Making a connection inside Unity <HashLink link="/?/docs/v2/development/multiplayer#making-a-connection-inside-unity" /></h2>

        <p>
          There are many ways in which you can handle the connection process, but here's an idea:
        </p>

        <ol>
          <li>Make an empty scene, optionally adding a loading screen</li>
          <li>Once the scene loads, call and await <code>Api.Initialize</code></li>
          <li>Access the Instance ID and other data (such as the user ID) and send it to your server</li>
          <li>Once your server responds, load the next scene (that will be your actual game)</li>
        </ol>

        <p>
          This way you can ensure that any scene after the first one will have a multiplayer connection ready and Dissonity initialized.
        </p>
        
        <p>
          You will need to code your own networking implementation or use an existing library. If you only need to send HTTPS requests, you can use the <code>Api.Proxy</code> subclass.
        </p>

        <BoxTip title="Multiplayer libraries">
          <p>
            Discord uses <a href="https://colyseus.io" target="_blank">Colyseus</a> in most of their networking examples. It could be a good starting point if you're using Node.js!
          </p>
        </BoxTip>

        <h2 id="activity-participants">Activity participants <HashLink link="/?/docs/v2/development/multiplayer#activity-participants" /></h2>

        <p>
          You can access the participants of an activity through a command or a subscription.
        </p>

        <CodeBlock language="csharp">{`using static Dissonity.Api;
using Dissonity.Models;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    async void Start()
    {
        // Assuming API is already initialized at this point

        // Command
        Participant[] participants = await Commands.GetInstanceConnectedParticipants();

        // Subscription
        await Subscribe.ActivityInstanceParticipantsUpdate(UpdateParticipants);
    }

    void UpdateParticipants(Participant[] participants)
    {
        // Do something when the participants change
    }
}
`}</CodeBlock>

        <h2>External links</h2>

        <ul>
          <li><a href="https://discord.com/developers/docs/activities/development-guides#networking" target="_blank">Networking</a></li>
          <li><a href="https://discord.com/developers/docs/activities/development-guides#multiplayer-experience" target="_blank">Multiplayer Experience</a></li>
        </ul>
        <Footer />
    </div>
  );
}

export default DocsPage