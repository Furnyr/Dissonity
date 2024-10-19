
// Global
interface Window {
    outsideDiscord: boolean
}

// Unity methods
declare const UTF8ToString: (str: any) => string;

// Unity plugin
declare const LibraryManager: { library: string };
declare const mergeInto: (arg1: string, arg2: Record<string, (...args: any[]) => void>) => void;