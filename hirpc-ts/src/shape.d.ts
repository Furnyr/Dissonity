
export default interface IShape {

    public connect(hash: string): void;
    
    public test(): Promise<string>;

    public addRpcListener(hash: string, listener: (message: RpcMessage) => void): void;

    public removeRpcListener(hash: string, listener: (message: RpcMessage) => void): void;

    public sendToRpc(hash: string, payload: unknown, opcode = Opcode.Frame): void;
}