namespace RushLanes
{
    public static class GameIds
    {
        public const string ProductName = "Rush Lanes";
        public const string PlayerTag = "Player";
        public const string PlayerObjectName = "Player";

        public const int LaneCount = 3;
        public const float LaneWidth = 2.4f;
        public const float TileLength = 14f;
        public const float GroundWidth = 8.4f;
        public const float PlayerHeight = 1.05f;

        public const float BaseSpeed = 10f;
        public const float MaxSpeed = 22f;
        public const float SpeedGainPerSecond = 0.12f;
        public const float JumpVelocity = 9.2f;
        public const float Gravity = 24f;
        public const float SlideDuration = 0.55f;

        public const int CoinScore = 10;
        public const int DistanceScoreDivisor = 2;
    }

    public enum GameState
    {
        Splash,
        Menu,
        Tutorial,
        Characters,
        Missions,
        Playing,
        Paused,
        GameOver
    }

    public enum ObstacleKind
    {
        Block,
        JumpBar,
        SlideGate
    }

    public enum PowerUpKind
    {
        Shield,
        Magnet,
        Boost
    }
}
