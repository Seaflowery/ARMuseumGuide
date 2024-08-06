using System;
using UnityEngine;
namespace Util
{
    public static class TimeManager
    {
        private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public static float fTimeScale = 1F;

        public static float TimeScale
        {
            get => fTimeScale;
            set => fTimeScale = value;
        }

        public static float DeltaTime => Time.unscaledDeltaTime * fTimeScale;

        public static float DeltaTimeUnScale => Time.unscaledDeltaTime;

        public static float FixedDeltaTime => Time.fixedUnscaledDeltaTime * fTimeScale;

        public static float FixedTimeUnScale => Time.fixedUnscaledDeltaTime;

        public static long Now()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000;
        }
    }
}