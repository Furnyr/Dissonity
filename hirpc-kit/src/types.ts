import HiRpcV0_0 from "../types/versions/v0_0";
import HiRpcV0_4 from "../types/index";

type StartWith<V extends string> = `${V}${string}`;

export type HiRpc<V extends string> =
    V extends StartWith<"0.4"> ? HiRpcV0_4 :
    HiRpcV0_0;