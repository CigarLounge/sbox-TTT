using Sandbox;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace TTT;

public enum SlotType
{
	Primary,
	Secondary,
	Melee,
	OffensiveEquipment,
	UtilityEquipment,
	Grenade,
}

public enum HoldType
{
	None,
	Pistol,
	Rifle,
	Shotgun,
	Carry,
	Fists
}

[GameResource( "Carriable", "carri", "TTT carriable template.", Icon = "inventory_2" )]
public class CarriableInfo : ItemInfo
{
	[Category( "Important" )]
	public SlotType Slot { get; set; } = SlotType.Primary;

	[Category( "Important" )]
	public bool Spawnable { get; set; } = false;

	[Category( "Important" )]
	public bool CanDrop { get; set; } = true;

	[JsonPropertyName( "viewmodel" )]
	[Title( "View Model" ), Category( "ViewModels" ), ResourceType( "vmdl" )]
	public string ViewModelPath { get; set; } = "";

	[JsonPropertyName( "handsmodel" )]
	[Title( "Hands Model" ), Category( "ViewModels" ), ResourceType( "vmdl" )]
	public string HandsModelPath { get; set; } = "";

	[Category( "WorldModels" )]
	public HoldType HoldType { get; set; } = HoldType.None;

	[JsonPropertyName( "worldmodel" )]
	[Title( "World Model" ), Category( "WorldModels" ), ResourceType( "vmdl" )]
	public string WorldModelPath { get; set; } = "";

	[Category( "Stats" )]
	public float DeployTime { get; set; } = 0.6f;

	[HideInEditor]
	[JsonPropertyName( "cached-handsmodel" )]
	public Model HandsModel { get; private set; }

	[HideInEditor]
	[JsonPropertyName( "cached-viewmodel" )]
	public Model ViewModel { get; private set; }

	[HideInEditor]
	[JsonPropertyName( "cached-worldmodel" )]
	public Model WorldModel { get; private set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		HandsModel = Model.Load( HandsModelPath );
		ViewModel = Model.Load( ViewModelPath );
		WorldModel = Model.Load( WorldModelPath );
	}
}

public abstract partial class Carriable : BaseCarriable, IEntityHint, IUse
{
	[Net, Local, Predicted]
	public TimeSince TimeSinceDeployed { get; private set; }

	public TimeSince TimeSinceDropped { get; private set; }

	public new Player Owner
	{
		get => (Player)base.Owner;
		set => base.Owner = value;
	}

	public BaseViewModel HandsModelEntity { get; private set; }
	public Player PreviousOwner { get; private set; }

	/// <summary>
	/// The text that will show up in the inventory slot.
	/// </summary>
	public virtual string SlotText => string.Empty;
	public bool IsActiveChild => Owner?.ActiveChild == this;
	public CarriableInfo Info { get; private set; }

	public override void Spawn()
	{
		base.Spawn();

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );

		if ( string.IsNullOrWhiteSpace( ClassName ) )
		{
			Log.Error( this + " doesn't have a Library name!" );
			return;
		}

		Info = GameResource.GetInfo<CarriableInfo>( ClassName );
		Model = Info.WorldModel;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		if ( !string.IsNullOrWhiteSpace( ClassName ) )
			Info = GameResource.GetInfo<CarriableInfo>( ClassName );
	}

	public override void ActiveStart( Entity entity )
	{
		EnableDrawing = true;

		if ( entity is Player player )
		{
			var animator = player.GetActiveAnimator();

			if ( animator is not null )
				SimulateAnimator( animator );
		}

		if ( IsLocalPawn )
		{
			CreateViewModel();
			CreateHudElements();

			ViewModelEntity?.SetAnimParameter( "deploy", true );
		}

		TimeSinceDeployed = 0;

		if ( Host.IsClient )
			return;

		if ( !Components.GetAll<DNA>().Any( ( dna ) => dna.TargetPlayer == Owner ) )
			Components.Add( new DNA( Owner ) );
	}

	public override void Simulate( Client client ) { }

	public override void FrameSimulate( Client client ) { }

	public virtual void RenderHud( in Vector2 screenSize ) { }

	public override void BuildInput( InputBuilder input ) { }

	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( Info.ViewModel is not null )
		{
			ViewModelEntity = new ViewModel
			{
				EnableViewmodelRendering = true,
				Model = Info.ViewModel,
				Owner = Owner,
				Position = Position
			};
		}

		if ( Info.HandsModel is not null )
		{
			HandsModelEntity = new BaseViewModel
			{
				EnableViewmodelRendering = true,
				Model = Info.HandsModel,
				Owner = Owner,
				Position = Position
			};

			HandsModelEntity.SetParent( ViewModelEntity, true );
		}
	}

	public override void CreateHudElements() { }

	public override void DestroyViewModel()
	{
		base.DestroyViewModel();

		HandsModelEntity?.Delete();
		HandsModelEntity = null;
	}

	public override bool CanCarry( Entity carrier )
	{
		if ( Owner is not null || carrier is not Player )
			return false;

		if ( carrier == PreviousOwner && TimeSinceDropped < 1f )
			return false;

		return true;
	}

	public override void OnCarryStart( Entity carrier )
	{
		base.OnCarryStart( carrier );

		// Bandaid fix for: https://github.com/Facepunch/sbox-issues/issues/1702
		if ( IsClient )
			Info = GameResource.GetInfo<CarriableInfo>( GetType() );

		PreviousOwner = Owner;
	}

	public override void OnCarryDrop( Entity dropper )
	{
		base.OnCarryDrop( dropper );

		TimeSinceDropped = 0;
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		base.SimulateAnimator( anim );

		anim.SetAnimParameter( "holdtype", (int)Info.HoldType );
	}

	bool IEntityHint.CanHint( Player player ) => Owner is null;

	bool IUse.OnUse( Entity user )
	{
		var player = (Player)user;
		player.Inventory.OnUse( this );

		return false;
	}

	bool IUse.IsUsable( Entity user ) => Owner is null && user is Player;

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotReload()
	{
		Info = GameResource.GetInfo<CarriableInfo>( ClassName );
	}
#endif
}
