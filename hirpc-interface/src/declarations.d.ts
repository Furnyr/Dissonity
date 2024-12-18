
// Global
interface Window {
    dso_needs_suffix: boolean
    dso_hirpc: any
    dso_connected: boolean
    dso_outside_discord: boolean
}

// Unity methods
declare const UTF8ToString: (str: any) => string;

// Unity plugin
declare const LibraryManager: { library: string };
declare const mergeInto: (arg1: string, arg2: Record<string, unknown>) => void;
declare const SendMessage = (object: string, method: string, data: string) => {}

// App loader
declare const createUnityInstance: (canvas: HTMLCanvasElement, config: Record<string, unknown>) => Promise<void>;