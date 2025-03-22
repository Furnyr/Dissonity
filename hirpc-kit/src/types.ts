import UnknownHiRpc from "../types/versions/v0_0";
import LatestHiRpc from "../types/index";

type StartWith<V extends string> = `${V}${string}`;

export type HiRpcShape<V extends string> =
    V extends StartWith<"0.5"> ? LatestHiRpc :
    UnknownHiRpc;