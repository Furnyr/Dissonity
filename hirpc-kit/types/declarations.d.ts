interface Window {
    Dissonity: {
        HiRpc: {
            default: {
                new (): unknown;
            };
        };
        BuildVariables: {
            default: {
                new (): unknown;
            };
        };
    };
    dso_hirpc: unknown;
    dso_build_variables: unknown;
}
interface SessionStorage {
    dso_outside_discord: "true" | "false" | null;
    dso_needs_prefix: "true" | "false" | null;
    dso_connected: "true" | "false" | null;
    dso_authenticated: "true" | "false" | null;
}
