import { HiRpcShape } from "./types";
export { HiRpcShape };

// import type { Opcode } from "../types/enums";
export enum RpcOpcode {
    Handshake = 0,
    Frame = 1,
    Close = 2,
    Hello = 3
};

/**
 * Prepare a hiRPC instance in `window.dso_hirpc`.
 * 
 * Call this as soon as possible, since hiRPC needs to listen to RPC activity from the beginning of the process to provide functionality.
 */
export async function setupHiRpc<V extends string>(_hiRpcVersion: V, bridgePath?: string): Promise<HiRpcShape<V>> {

    //? Web environment
    if (typeof window == "undefined") {

        throw new Error("Cannot load hiRPC Module outside of a web environment");
    }

    return new Promise((resolve, reject) => {

        //? Instance already deployed
        if (typeof window.dso_hirpc == "object") {
            resolve(window.dso_hirpc as HiRpcShape<V>);
            return;   
        }

        tryDirectImport()
        .then(module => {
            resolve(module as HiRpcShape<V>)
        })
        .catch(err => {
            reject(err);
        })

        function tryDirectImport() {

            // Hierarchy of preferences: Kit input > Session storage
            const bridgeImport = bridgePath ?? sessionStorage.getItem("dso_bridge");

            if (!bridgeImport) {
                throw new Error("You must specify a bridge path using the 'bridgePath' parameter or the 'dso_bridge' session storage item.");
            }

            return new Promise((resolve, reject) => {

                import(`${bridgeImport}dissonity_hirpc.js` as string)
                .then(() => {
    
                    import(`${bridgeImport}dissonity_build_variables.js` as string)
                    .then(() => {
    
                        mountInstance();
                    
                        resolve(window.dso_hirpc as HiRpcShape<V>);
                    })
                    .catch(err => {
                        reject(err);
                    });
                })
                .catch(err => {
                    reject(err);
                })
            });
        }

        function mountInstance() {

            // Note: The instance is automatically defined in the window in newer hiRPC versions
            const instance = new window.Dissonity.HiRpc.default();

            if (window.dso_hirpc != instance) {
                window.dso_hirpc = instance;
            }

            clearRpcSessionStorage();
        }
    
        function clearRpcSessionStorage() {

            const hiRpc = window.dso_hirpc as HiRpcShape<V>;
            const query = hiRpc.getQueryObject();

            // Meaningless casts to provoke an error if the SessionStorage property changes.
            sessionStorage.removeItem("dso_connected" as NonNullable<SessionStorage["dso_connected"]>);
            sessionStorage.removeItem("dso_authenticated" as NonNullable<SessionStorage["dso_authenticated"]>);

            sessionStorage.setItem("dso_instance_id", query.instance_id as NonNullable<SessionStorage["dso_instance_id"]>);
        }
    });
}

/**
 * Load an HTML file as the activity iframe.
 */
export function loadIframe(src: string, id: string) {

    const iframe = document.createElement("iframe");

    iframe.id = id;
    iframe.src = src;
    iframe.height = "100vh";
    iframe.width = "100vw";

    iframe.style.display = "block";
    iframe.style.border = "0px";
    iframe.style.overflow = "hidden";
    iframe.style.height = "100vh";
    iframe.style.width = "100vw";

    document.body.appendChild(iframe);
}