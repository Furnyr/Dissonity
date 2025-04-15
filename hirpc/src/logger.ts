
export function log(message?: any) {
    console.log(`%c[DissonityHiRpc]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
}

export function logError(message?: any) {
    console.error(`%c[DissonityHiRpc]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
}