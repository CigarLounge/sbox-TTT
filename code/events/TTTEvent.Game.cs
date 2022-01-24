namespace TTT.Events
{
    public static partial class TTTEvent
    {
        public static class Game
        {
            /// <summary>
            /// Should be used to precache models and stuff.
            /// </summary>
            public const string Precache = "TTT.game.precache";

            /// <summary>
            /// Called everytime the round changes.
            /// <para>Event is passed the <strong><see cref="TTT.Rounds.BaseRound"/></strong> instance of the old round.</para>
            /// <para>Event is passed the <strong><see cref="TTT.Rounds.BaseRound"/></strong> instance of the new round.</para>
            /// </summary>
            public const string RoundChange = "TTT.game.roundchange";

            /// <summary>
            /// Updates when the map images are networked.
            /// </summary>
            public const string MapImagesChange = "TTT.game.mapimagechange";
        }
    }
}
