using Sandbox;
using System.Collections.Generic;
using System.Linq;

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

[Title( "Carriable" ), Icon( "luggage" )]
public abstract partial class Carriable : AnimatedEntity, IEntityHint, IUse
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
	public CarriableInfo Info { get; private set; }
	public Player PreviousOwner { get; private set; }
	public BaseViewModel ViewModelEntity { get; protected set; }

	/// <summary>
	/// Return the entity we should be spawning particles from.
	/// </summary>
	public virtual ModelEntity EffectEntity => ViewModelEntity.IsValid() && Camera.FirstPersonViewer == Owner ? ViewModelEntity : this;

	/// <summary>
	/// The text that will show up in the inventory slot.
	/// </summary>
	public virtual string SlotText => string.Empty;

	/// <summary>
	/// Prompt(s) that appear at the bottom of the user's screen.
	/// An InputButton glyph and a piece of text.
	/// If the given text string is null or empty, it will not be displayed.
	/// </summary>
	public virtual List<UI.BindingPrompt> BindingPrompts => new();

	public bool IsActive => Owner?.ActiveCarriable == this;

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "interactable" );
		PhysicsEnabled = true;
		UsePhysicsCollision = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		if ( ClassName.IsNullOrEmpty() )
		{
			Log.Error( this + " doesn't have a class name!" );
			return;
		}

		Info = GameResource.GetInfo<CarriableInfo>( ClassName );
		Model = Info.WorldModel;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		if ( !ClassName.IsNullOrEmpty() )
			Info = GameResource.GetInfo<CarriableInfo>( ClassName );
	}

	public virtual void ActiveStart( Player player )
	{
		EnableDrawing = true;

		if ( IsLocalPawn )
		{
			CreateViewModel();
			CreateHudElements();

			ViewModelEntity?.SetAnimParameter( "deploy", true );
		}

		TimeSinceDeployed = 0;

		if ( !Game.IsServer )
			return;

		if ( !Components.GetAll<DNA>().Any( ( dna ) => dna.TargetPlayer == Owner ) )
			Components.Add( new DNA( Owner ) );
	}

	public virtual void ActiveEnd( Player player, bool dropped )
	{
		if ( !dropped )
			EnableDrawing = false;

		if ( Game.IsClient )
		{
			DestroyViewModel();
			DestroyHudElements();
		}
	}

	public override void Simulate( IClient client ) { }

	public override void FrameSimulate( IClient client ) { }

	public override void BuildInput() { }

	public virtual bool CanCarry( Player carrier )
	{
		if ( Owner is not null )
			return false;

		if ( carrier == PreviousOwner && TimeSinceDropped < 1f )
			return false;

		return true;
	}

	public virtual void OnCarryStart( Player carrier )
	{
		// Bandaid fix for: https://github.com/Facepunch/sbox-issues/issues/1702
		if ( Game.IsClient )
			Info ??= GameResource.GetInfo<CarriableInfo>( GetType() );

		if ( !Game.IsServer )
			return;

		Owner = carrier;
		EnableAllCollisions = false;
		EnableDrawing = false;
	}

	public virtual void OnCarryDrop( Player dropper )
	{
		PreviousOwner = dropper;

		if ( !Game.IsServer )
			return;

		Owner = null;
		EnableDrawing = true;
		EnableAllCollisions = true;
		TimeSinceDropped = 0;
		Position = dropper.WorldSpaceBounds.Center;
	}

	public override Sound PlaySound( string soundName, string attachment )
	{
		if ( Owner.IsValid() )
			return Owner.PlaySound( soundName, attachment );

		return base.PlaySound( soundName, attachment );
	}

	public virtual void SimulateAnimator( CitizenAnimationHelper anim )
	{
		anim.HoldType = Info.HoldType;
		anim.AimBodyWeight = 1.0f;
		anim.Handedness = 0;
	}

	/// <summary>
	/// Create the viewmodel. You can override this in your base classes if you want
	/// to create a certain viewmodel entity.
	/// </summary>
	public virtual void CreateViewModel()
	{
		Game.AssertClient();

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

	/// <summary>
	/// We're done with the viewmodel - delete it
	/// </summary>
	protected virtual void DestroyViewModel()
	{
		ViewModelEntity?.Delete();
		ViewModelEntity = null;
		HandsModelEntity?.Delete();
		HandsModelEntity = null;
	}

	protected virtual void CreateHudElements() { }

	protected virtual void DestroyHudElements() { }

	protected override void OnDestroy()
	{
		base.OnDestroy();

		DestroyViewModel();
		DestroyHudElements();
	}

	bool IEntityHint.CanHint( Player player ) => Owner is null;

	bool IUse.OnUse( Entity user )
	{
		if ( user is Player player )
			player.Inventory.OnUse( this );

		return false;
	}

	bool IUse.IsUsable( Entity user ) => Owner is null && user is Player player && player.IsAlive;

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotload()
	{
		Info = GameResource.GetInfo<CarriableInfo>( ClassName );
	}
#endif
}
