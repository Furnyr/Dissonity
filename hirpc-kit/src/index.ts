import { HiRpc } from "./types";

export async function setupHiRpc<V extends string>(_hiRpcVersion: V): Promise<HiRpc<V>> {

    return new Promise((resolve, reject) => {

        //? Instance already deployed
        if (typeof window.dso_hirpc == "object") {
            resolve(window.dso_hirpc as HiRpc<V>);       
        }

        import("dso_proxy_bridge/dissonity_hirpc.js" as string)
        .then(() => {

            import("dso_proxy_bridge/dissonity_build_variables.js" as string)
            .then(() => {

                window.dso_hirpc = new window.Dissonity.HiRpc.default();
            
                resolve(window.dso_hirpc as HiRpc<V>);
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
                    
                    window.dso_hirpc = new window.Dissonity.HiRpc.default();
                
                    resolve(window.dso_hirpc as HiRpc<V>);
                })
                .catch(err => {
                    reject(err);
                });
            })
            .catch(err => {
                reject(err);
            })
        });
    })
}