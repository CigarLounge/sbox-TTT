using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeAttribute : LibraryAttribute
    {
        public NodeAttribute(string name) : base("node_" + name)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SpawnableAttribute : Attribute
    {
        public string Categorie = "General";

        public SpawnableAttribute(string categorie = null) : base()
        {
            Categorie = categorie ?? Categorie;
        }
    }

    public abstract class Node : Modal
    {
        public string LibraryName { get; set; }
        public List<Node> NextNodes { get; set; } = new();
        public List<int> ConnectPositions { get; set; } = new();

        public List<NodeSetting> NodeSettings = new();
        public StackNode StackNode;

        public Node(StackNode stackNode) : base()
        {
            StackNode = stackNode;
            LibraryName = Utils.GetLibraryName(GetType());
            StackNode.NodeReference = LibraryName;

            Header.DragHeader.IsFreeDraggable = true;
            Header.DragHeader.IsLocked = false;

            StyleSheet.Load("/ui/visual-programming/nodes/node/Node.scss");

            AddClass("node");
            AddClass("box-shadow");
        }

        public static NodeAttribute GetAttribute<T>() where T : Node => Library.GetAttribute(typeof(T)) as NodeAttribute;

        public NodeAttribute GetAttribute() => Library.GetAttribute(GetType()) as NodeAttribute;

        public T AddSetting<T>() where T : NodeSetting, new()
        {
            T nodeSetting = new();
            nodeSetting.Node = this;

            Content.AddChild(nodeSetting);
            NodeSettings.Add(nodeSetting);

            return nodeSetting;
        }

        public override void Delete(bool immediate = false)
        {
            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                if (nodeSetting.Output != null)
                {
                    nodeSetting.Output.ConnectionPoint.ConnectionWire?.Delete(true);
                }

                if (nodeSetting.Input != null)
                {
                    nodeSetting.Input.ConnectionPoint.ConnectionWire?.Delete(true);
                }
            }

            base.Delete(immediate);
        }

        protected override void OnRightClick(Sandbox.UI.MousePanelEvent e)
        {
            Window.Instance?.RemoveNode(this);

            Delete(true);

            base.OnRightClick(e);
        }

        private Node GetConnectedNode(NodeConnectionPoint connectionPoint, out int index)
        {
            index = -1;

            NodeConnectionWire connectionWire = connectionPoint.ConnectionWire;

            if (connectionWire == null)
            {
                return null;
            }

            NodeConnectionPoint connectedPoint = connectionWire.EndPoint;

            if (connectedPoint == null)
            {
                return null;
            }

            index = connectedPoint.GetSettingsIndex();

            return connectedPoint.Node;
        }

        public bool HasInput()
        {
            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                if (nodeSetting.Input.ConnectionPoint.ConnectionWire != null)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool Build(params object[] input)
        {
            NextNodes.Clear();
            StackNode.NextNodes.Clear();
            StackNode.ConnectPositions.Clear();

            for (int i = 0; i < NodeSettings.Count; i++)
            {
                NodeSetting nodeSetting = NodeSettings[i];

                if (nodeSetting.Output == null)
                {
                    continue;
                }

                Node connectedNode = GetConnectedNode(nodeSetting.Output.ConnectionPoint, out int connectPositionIndex);

                StackNode.ConnectPositions.Add(connectPositionIndex);

                if (connectedNode == null)
                {
                    continue;
                }

                NextNodes.Add(connectedNode);
                StackNode.NextNodes.Add(connectedNode.StackNode);
            }

            StackNode.SetPos(Box.Rect.left, Box.Rect.top);

            object[] arr;

            try
            {
                arr = StackNode.Test(input);
            }
            catch (Exception e)
            {
                HighlightError();

                if (e is NodeStackException)
                {
                    Log.Warning($"Error in node '{GetType()}': ({e.Source}): {e.Message}\n{e.StackTrace}");

                    return false;
                }

                throw;
            }

            bool errors = false;

            for (int i = 0; i < NextNodes.Count; i++)
            {
                Node node = NextNodes[i];

                try
                {
                    if (!node.Build(arr.Length > i ? arr[i] : null))
                    {
                        errors = true;
                    }
                }
                catch (Exception e)
                {
                    if (e is not NodeStackException)
                    {
                        node.HighlightError();
                    }

                    throw;
                }
            }

            return !errors;
        }

        public virtual void HighlightError()
        {
            AddClass("error");
        }

        public virtual void RemoveHighlights()
        {
            RemoveClass("error");
        }

        public void ConnectWithNode(Node node, int index, int maxIndex)
        {
            if (node == this)
            {
                return;
            }

            while (index <= maxIndex && ConnectPositions[index] == -1)
            {
                index++;

                if (index == maxIndex)
                {
                    return;
                }
            }

            NodeConnectionWire nodeConnectionWire = NodeConnectionWire.Create();

            NodeConnectionStartPoint startPoint = NodeSettings[index].Output.ConnectionPoint;
            startPoint.ConnectionWire = nodeConnectionWire;
            nodeConnectionWire.StartPoint = startPoint;

            NodeConnectionEndPoint endPoint = node.NodeSettings[ConnectPositions[index]].Input.ConnectionPoint;
            endPoint.ConnectionWire = nodeConnectionWire;
            nodeConnectionWire.EndPoint = endPoint;
        }

        public virtual Dictionary<string, object> GetJsonData(List<Node> proceedNodes = null)
        {
            if (proceedNodes != null)
            {
                proceedNodes.Add(this);
            }

            List<Dictionary<string, object>> nextNodesJsonList = new();

            NextNodes.Clear();
            ConnectPositions.Clear();

            for (int i = 0; i < NodeSettings.Count; i++)
            {
                NodeSetting nodeSetting = NodeSettings[i];

                if (nodeSetting.Output == null)
                {
                    continue;
                }

                Node connectedNode = GetConnectedNode(nodeSetting.Output.ConnectionPoint, out int connectPositionIndex);

                ConnectPositions.Add(connectPositionIndex);

                if (connectedNode == null)
                {
                    continue;
                }

                NextNodes.Add(connectedNode);
            }

            foreach (Node node in NextNodes)
            {
                nextNodesJsonList.Add(node.GetJsonData(proceedNodes));
            }

            return new Dictionary<string, object>()
            {
                ["LibraryName"] = LibraryName,
                ["ConnectPositions"] = ConnectPositions,
                ["NextNodes"] = nextNodesJsonList,
                ["PosX"] = Box.Rect.left,
                ["PosY"] = Box.Rect.top
            };
        }

        public virtual void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            Window.Instance.AddNode(this);

            jsonData.TryGetValue("ConnectPositions", out object connectPosition);

            if (connectPosition != null)
            {
                ConnectPositions = JsonSerializer.Deserialize<List<int>>(((JsonElement) connectPosition).GetRawText());
            }

            jsonData.TryGetValue("NextNodes", out object nextNodes);

            if (nextNodes != null)
            {
                JsonElement nextNodesElement = (JsonElement) nextNodes;
                List<Dictionary<string, object>> nextNodesList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(nextNodesElement.GetRawText());

                for (int i = 0; i < nextNodesList.Count; i++)
                {
                    Node node = GetNodeFromJsonData<Node>(nextNodesList[i]);

                    if (node == null)
                    {
                        continue;
                    }

                    NextNodes.Add(node);
                    ConnectWithNode(node, i, nextNodesList.Count - 1);
                }
            }

            jsonData.TryGetValue("PosX", out object posX);

            if (posX != null)
            {
                Style.Left = Sandbox.UI.Length.Pixels(float.Parse(posX.ToString()));
            }

            jsonData.TryGetValue("PosY", out object posY);

            if (posY != null)
            {
                Style.Top = Sandbox.UI.Length.Pixels(float.Parse(posY.ToString()));
            }
        }

        public static T GetNodeFromJsonData<T>(Dictionary<string, object> jsonData) where T : Node
        {
            jsonData.TryGetValue("LibraryName", out object libraryName);

            if (libraryName == null)
            {
                return null;
            }

            Type type = Utils.GetTypeByLibraryName<T>(libraryName.ToString());

            if (type == null)
            {
                return null;
            }

            T node = Utils.GetObjectByType<T>(type);

            if (node == null)
            {
                return null;
            }

            node.LoadFromJsonData(jsonData);
            node.RemoveHighlights();

            return node;
        }
    }
}
