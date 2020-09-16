using System;

namespace Examples.Volleyball
{
    public class Tags
    {
        public const string Net = "Net";
        public const string LeftPlayer = "LeftPlayer";
        public const string RightPlayer = "RightPlayer";
        public const string Wall = "Wall";
        public const string LeftFloor = "LeftFloor";
        public const string RightFloor = "RightFloor";

        public static float GetPlayerSign(string tag)
        {
            return IsLeftPlayer(tag) ? -1f : 1f;
        }

        public static string GetOtherPlayer(string tag)
        {
            return IsLeftPlayer(tag) ? RightPlayer : LeftPlayer;
        }

        public static bool IsLeftPlayer(string tag)
        {
            switch (tag)
            {
                case LeftPlayer:
                    return true;
                case RightPlayer:
                    return false;
                default:
                    throw new ArgumentException($"Invalid player tag: {tag}");
            }
        }
    }
}
