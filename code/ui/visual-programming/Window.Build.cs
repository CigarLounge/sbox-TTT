using System;
using System.Text.Json;

using TTT.VisualProgramming;

namespace TTT.UI.VisualProgramming
{
    public partial class Window
    {
        public void Build()
        {
            BuildButton.Icon = "hourglass_empty";

            MainNode.StackNode.Reset();

            bool hasError = false;

            foreach (Node node in Nodes)
            {
                node.RemoveHighlights();

                if (!node.HasInput() && node != MainNode)
                {
                    node.HighlightError();

                    hasError = true;
                }
            }

            if (hasError)
            {
                return;
            }

            try
            {
                Log.Debug("Building NodeStack");

                if (!MainNode.Build())
                {
                    return;
                }

                Log.Debug("Uploading NodeStack");

                NodeStack.UploadStack(JsonSerializer.Serialize(MainNode.StackNode.GetJsonData()));
            }
            catch (Exception e)
            {
                Log.Error(e);

                BuildButton.Icon = "play_arrow";
            }
        }
    }
}
