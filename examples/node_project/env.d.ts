
declare namespace NodeJS {
    interface ProcessEnv {
        PUBLIC_CLIENT_ID: string,
        CLIENT_SECRET: string,
        PORT: string,
        COLYSEUS: string
    }
}