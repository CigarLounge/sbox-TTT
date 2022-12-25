using Sandbox;
using System.Text.Json.Serialization;

namespace TTT;

[GameResource( "Carriable", "carri", "TTT carriable template.", Icon = "inventory_2" )]
public class CarriableInfo : ItemInfo
{
	[Category( "Important" )]
	public SlotType Slot { get; set; } = SlotType.Primary;

	[Category( "Important" )]
	public bool Spawnable { get; set; } = false;

	[Category( "Important" )]
	public bool CanDrop { get; set; } = true;

	[Title( "View Model" ), Category( "ViewModels" ), ResourceType( "vmdl" )]
	public string ViewModelPath { get; set; } = "";

	[Title( "Hands Model" ), Category( "ViewModels" ), ResourceType( "vmdl" )]
	public string HandsModelPath { get; set; } = "";

	[Category( "WorldModels" )]
	public CitizenAnimationHelper.HoldTypes HoldType { get; set; } = CitizenAnimationHelper.HoldTypes.None;

	[Title( "World Model" ), Category( "WorldModels" ), ResourceType( "vmdl" )]
	public string WorldModelPath { get; set; } = "";

	[Category( "Stats" )]
	public float DeployTime { get; set; } = 0.6f;

	[HideInEditor]
	[JsonIgnore]
	public Model HandsModel { get; private set; }

	[HideInEditor]
	[JsonIgnore]
	public Model ViewModel { get; private set; }

	[HideInEditor]
	[JsonIgnore]
	public Model WorldModel { get; private set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		HandsModel = Model.Load( HandsModelPath );
		ViewModel = Model.Load( ViewModelPath );
		WorldModel = Model.Load( WorldModelPath );
	}
}
