using Sandbox;

namespace TTT;

public interface IEntityHint
{
	/// <summary>
	/// The max viewable distance of the hint.
	/// </summary>
	float HintDistance => Player.USE_DISTANCE;

	/// <summary>
	/// Whether or not we can show the UI hint.
	/// </summary>
	bool CanHint( Player player ) => true;

	/// <summary>
	/// The hint we should display.
	/// </summary>
	UI.EntityHintPanel DisplayHint( Player player )
	{
		return new UI.Hint( (this as Entity).ClassInfo.Title );
	}

	/// <summary>
	/// Occurs on each tick if the hint is active.
	/// </summary>
	void Tick( Player player ) { }
}
