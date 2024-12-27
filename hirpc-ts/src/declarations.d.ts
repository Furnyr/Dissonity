
// Global
interface Window {
    ["Dissonity.HiRpc"]: any;           // hiRPC main class
    ["Dissonity.BuildVariables"]: any;  // Build variables class
    dso_hirpc: any;                     // hiRPC instance
    dso_build_variables: any;           // Build variables instance
    dso_needs_suffix: boolean;          // True is .proxy is needed
    dso_connected: boolean;             // True after handshake
    dso_outside_discord: boolean;       // True if inside browser
}