import { HiRpcShape } from "./types";

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
export async function setupHiRpc<V extends string>(_hiRpcVersion: V): Promise<HiRpcShape<V>> {

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

        import("dso_proxy_bridge/dissonity_hirpc.js" as string)
        .then(() => {

            import("dso_proxy_bridge/dissonity_build_variables.js" as string)
            .then(() => {

                // In this case, outside_discord is closely related to the path
                sessionStorage.setItem("dso_needs_prefix", "true" as NonNullable<SessionStorage["dso_needs_prefix"]>);
                sessionStorage.setItem("dso_outside_discord", "false" as NonNullable<SessionStorage["dso_outside_discord"]>);

                // Note: The instance is automatically defined in the window in newer hiRPC versions
                const instance = new window.Dissonity.HiRpc.default();

                if (window.dso_hirpc != instance) {
                    window.dso_hirpc = instance;
                }

                clearRpcSessionStorage();
            
                resolve(window.dso_hirpc as HiRpcShape<V>);
            })
            .catch(err => {
                reject(err);
            })
        })
        .catch(_ => {

            import("dso_bridge/dissonity_hirpc.js" as string)
            .then(() => {

                import("dso_bridge/dissonity_build_variables.js" as string)
                .then(() => {

                    // In this case, outside_discord is closely related to the path
                    sessionStorage.setItem("dso_needs_prefix", "false" as NonNullable<SessionStorage["dso_needs_prefix"]>);
                    sessionStorage.setItem("dso_outside_discord", "true" as NonNullable<SessionStorage["dso_outside_discord"]>);
                    
                    // Note: The instance is automatically defined in the window in newer hiRPC versions
                    const instance = new window.Dissonity.HiRpc.default();

                    if (window.dso_hirpc != instance) {
                        window.dso_hirpc = instance;
                    }

                    clearRpcSessionStorage();
                
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

    const confirmedNeedsPrefix = sessionStorage.getItem("dso_needs_prefix") as SessionStorage["dso_needs_prefix"] == "true";
    if (confirmedNeedsPrefix && !src.startsWith("./") && !src.startsWith(".proxy/")) {
        src = ".proxy/" + src;
    }

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