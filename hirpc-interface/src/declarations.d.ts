// Unity methods
declare const UTF8ToString: (str: any) => string;

// Unity plugin
declare const LibraryManager: { library: string };
declare const mergeInto: (arg1: string, arg2: Record<string, unknown>) => void;
declare const SendMessage = (object: string, method: string, data: string) => {}

// App loader
declare const createUnityInstance: (canvas: HTMLCanvasElement, config: Record<string, unknown>) => Promise<void>;

interface Window {
    dso_expand_canvas: (() => void) | undefined;  // Function added when using Max resolution
}

declare const DISSONITY_CHANNEL: string;