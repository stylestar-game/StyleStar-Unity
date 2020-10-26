namespace StyleStar
{
    public class HitResult
    {
        public bool WasHit;
        public float Difference; // In milliseconds, + is early and - is late
        public bool WasProcessed = false;
    }

    public enum HitGrade
    {
        // All timing is guesses based on DDR, which is actually probably too strict
        Bad,    // 8 frames @ 60fps (133.3ms)
        Good,   // 6 frames @ 60fps (100ms)
        Great,  // 4 frames @ 60fps (66.6ms)
        Perfect // 2 frames @ 60fps (33.3ms)
    }

    public static class Timing
    {
        public static readonly float MissFlag = -999.0f;
    }

    public static class NoteTiming
    {
        public static readonly float Bad = 1 / 60.0f * 8;
        public static readonly float Good = 1 / 60.0f * 6;
        public static readonly float Great = 1 / 60.0f * 4;
        public static readonly float Perfect = 1 / 60.0f * 2;

        public static readonly double BeatTolerance = 0.05; // This is the window that hold note beats will count

        public static readonly double AutoTolerance = 0.01;

        public static readonly float Shuffle = 1 / 60.0f * 8;
        public static readonly int ShuffleVelocityThreshold = 20;
    }

    public static class MotionTiming
    {
        // These need to be like 10 times wider
        public static readonly float Miss = 1 / 60.0f * 40;
        public static readonly float EarlyPerfect = 1 / 60.0f * 2;
        public static readonly float Perfect = 1 / 60.0f * 10;
        public static readonly float JumpPerfectCheck = 1 / 60.0f * 2;
    }

    public enum HitState
    {
        Unknown,
        Miss,
        Hit
    }
}
