
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
        private static extern void DsoLog(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoWarn(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoError(string stringifiedMessage);
#endif

#if !UNITY_WEBGL
        private static void DsoLog(string _) {}
        private static void DsoWarn(string _) {}
        private static void DsoError(string _) {}
#endif

        /// <summary>
        /// Log a message adding [Dissonity] in the browser console.
        /// </summary>
        public static void DissonityLog(object message)
        {
            if (Api.isEditor)
            {
                Debug.Log($"[Dissonity] {message}");
            }

            else
            {
                DsoLog($"{message}");
            }
        }

        /// <summary>
        /// Log a message adding [Dissonity] in the browser console.
        /// </summary>
        public static void DissonityLogWarning(object message)
        {
            if (Api.isEditor)
            {
                Debug.LogWarning($"[Dissonity] {message}");
            }

            else
            {
                DsoWarn($"{message}");
            }
        }

        /// <summary>
        /// Log a message adding [Dissonity] in the browser console.
        /// </summary>
        public static void DissonityLogError(object message)
        {
            if (Api.isEditor)
            {
                Debug.LogError($"[Dissonity] {message}");
            }

            else
            {
                DsoError($"{message}");
            }
        }

        /// <summary>
        /// Get a snowflake to use as a placeholder.
        /// </summary>
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