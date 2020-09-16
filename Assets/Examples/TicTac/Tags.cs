using System;

namespace Examples.TicTac
{
    public class Tags
    {
        public const string PlayerO = "PlayerO";
        public const string PlayerX = "PlayerX";

        public static string GetOtherPlayer(string tag)
        {
            switch (tag)
            {
                case PlayerO:
                    return PlayerX;
                case PlayerX:
                    return PlayerO;
                default:
                    throw new ArgumentException($"Invalid player tag: {tag}");
            }
        }
    }
}