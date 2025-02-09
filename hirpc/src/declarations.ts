
// Global
interface Window {
    Dissonity: {
        HiRpc: {
            default: { new(): unknown } // hiRPC main class
        }
        BuildVariables: {
            default: { new(): unknown } // Build variables class [import needed]
        }
    };
    dso_hirpc: unknown;             // hiRPC instance
    dso_build_variables: unknown;   // Build variables instance
}

interface SessionStorage {
    dso_outside_discord:    "true" | "false" | null;  // True if inside browser
    dso_needs_prefix:       "true" | "false" | null;  // True if .proxy is needed
    dso_connected:          "true" | "false" | null;  // True after handshake
    dso_authenticated:      "true" | "false" | null;  // True after authentication
}