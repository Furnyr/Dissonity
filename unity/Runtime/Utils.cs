
using System;
using UnityEngine;

namespace Dissonity
{
    public static class Utils
    {
        private static long epoch = 1420070400000;
        private static long _now = 0;
        private static long _increment = 0;

        public static void DissonityLog(object message)
        {
            Debug.Log($"[Dissonity]: {message}");
        }

        public static void DissonityLogWarning(object message)
        {
            Debug.LogWarning($"[Dissonity]: {message}");
        }

        public static void DissonityLogError(object message)
        {
            Debug.LogError($"[Dissonity]: {message}");
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