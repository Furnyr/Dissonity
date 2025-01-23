
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Dissonity
{
    public static class Utils
    {
        private static long epoch = 1420070400000;
        private static long _now = 0;
        private static long _increment = 0;

                //# HIRPC INTERFACE - - - - -
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void DissonityLog(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DissonityWarn(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DissonityError(string stringifiedMessage);
#endif

#if !UNITY_WEBGL
        private static void DissonityLog(string _) {}
        private static void DissonityWarn(string _) {}
        private static void DissonityError(string _) {}
#endif

        public static void DissonityLog(object message)
        {
            if (Api.isEditor)
            {
                Debug.Log($"[Dissonity] {message}");
            }

            else
            {
                DissonityLog($"{message}");
            }
        }

        public static void DissonityLogWarning(object message)
        {
            if (Api.isEditor)
            {
                Debug.LogWarning($"[Dissonity] {message}");
            }

            else
            {
                DissonityWarn($"{message}");
            }
        }

        public static void DissonityLogError(object message)
        {
            if (Api.isEditor)
            {
                Debug.LogError($"[Dissonity] {message}");
            }

            else
            {
                DissonityError($"{message}");
            }
        }

        public static long GetMockSnowflake()
        {
            var current = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (current != _now)
            {
                _now = current;
                _increment = 0;
            }

            else _increment++;

            long timestamp = current - epoch;

            long snowflake = timestamp << 23 | 1 << 13 | _increment;

            return snowflake;
        }
    }
}