using Sandbox;
using Sandbox.UI;

namespace TTT;

public interface IEntityHint
{
	/// <summary>
	/// The max viewable distance of the hint.
	/// </summary>
	float HintDistance => Player.UseDistance;

	/// <summary>
	/// Whether or not the entity should show a glow outline.
	/// </summary>
	bool ShowGlow => true;

	/// <summary>
	/// Whether or not we can show the UI hint.
	/// </summary>
	bool CanHint( Player player ) => true;

	/// <summary>
	/// The hint we should display.
	/// </summary>
	Panel DisplayHint( Player player )
	{
		return new UI.Hint() { HintText = DisplayInfo.For( (Entity)this ).Name };
	}

	/// <summary>
	/// Occurs on each tick if the hint is active.
	/// </summary>
	void Tick( Player player ) { }
}
