using System;

namespace Examples.TicTac
{
    public class Tags
    {
        public const string PLAYER_O = "PlayerO";
        public const string PLAYER_X = "PlayerX";

        public static string GetOtherPlayer(string tag)
        {
            switch (tag)
            {
                case PLAYER_O:
                    return PLAYER_X;
                case PLAYER_X:
                    return PLAYER_O;
                default:
                    throw new ArgumentException($"Invalid player tag: {tag}");
            }
        }
    }
}