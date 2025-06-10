import UnknownHiRpc from "../types/versions/v0_0";
import HiRpcV0_5 from "../types/versions/v0_5";
import LatestHiRpc from "../types/index";

type StartWith<V extends string> = `${V}${string}`;

export type HiRpcShape<V extends string> =
    V extends StartWith<"0.5"> ? HiRpcV0_5 :
    V extends StartWith<"0.6"> ? LatestHiRpc :
    UnknownHiRpc;