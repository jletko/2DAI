using System;

namespace Examples.Volleyball
{
    public class Tags
    {
        public const string NET = "Net";
        public const string LEFT_PLAYER = "LeftPlayer";
        public const string RIGHT_PLAYER = "RightPlayer";
        public const string WALL = "Wall";
        public const string LEFT_FLOOR = "LeftFloor";
        public const string RIGHT_FLOOR = "RightFloor";

        public static float GetPlayerSign(string tag)
        {
            return IsLeftPlayer(tag) ? -1f : 1f;
        }

        public static string GetOtherPlayer(string tag)
        {
            return IsLeftPlayer(tag) ? RIGHT_PLAYER : LEFT_PLAYER;
        }

        public static bool IsLeftPlayer(string tag)
        {
            switch (tag)
            {
                case LEFT_PLAYER:
                    return true;
                case RIGHT_PLAYER:
                    return false;
                default:
                    throw new ArgumentException($"Invalid player tag: {tag}");
            }
        }
    }
}
