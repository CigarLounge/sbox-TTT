namespace TTT.VisualProgramming
{
    [StackNode("main")]
    public partial class AllPlayersStackNode : StackNode
    {
        public AllPlayersStackNode() : base()
        {

        }

        public override object[] Test(params object[] input)
        {
            return new object[]
            {
                Utils.GetPlayers()
            };
        }
    }
}
