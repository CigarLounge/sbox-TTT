using TTT.UI;

namespace TTT;

public interface IEntityHint
{
	/// <summary>
	/// The max viewable distance of the hint.
	/// </summary>
	float HintDistance => Player.INTERACT_DISTANCE;

	/// <summary>
	/// The text to display on the hint each tick.
	/// </summary>
	string TextOnTick { get; }


	/// <summary>
	/// The sub text to display on the hint each tick.
	/// </summary>
	string SubTextOnTick => "";

	/// <summary>
	/// Whether or not we can show the UI hint.
	/// </summary>
	bool CanHint( Player player );

	/// <summary>
	/// The hint we should display.
	/// </summary>
	EntityHintPanel DisplayHint( Player player );

	/// <summary>
	/// Occurs on each tick if the hint is active.
	/// </summary>
	void Tick( Player player ) { }
}
