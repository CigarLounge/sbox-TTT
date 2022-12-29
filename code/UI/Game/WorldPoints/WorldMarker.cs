using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class WorldMarker : Panel
{
	private readonly string _iconPath;
	private readonly Func<string> _getText;
	private readonly Func<Vector3> _getPosition;
	private readonly Func<bool> _deletePredicate;

	private Vector3 _position;
	private string _text;

	/// <summary>
	/// 2D marker of an icon with text below, visible anywhere on the map.
	/// Give it an icon path, the text and position of the marker (which update on each tick), and the condition for it to delete itself.
	/// </summary>
	public WorldMarker( string iconPath, Func<string> getText, Func<Vector3> getPosition, Func<bool> deletePredicate = null )
	{
		_iconPath = iconPath;
		_getText = getText;
		_deletePredicate = deletePredicate;
		_getPosition = getPosition;
	}

	public override void Tick()
	{
		if ( _deletePredicate is not null && _deletePredicate() )
			Delete();

		if ( IsDeleting )
			return;

		_position = _getPosition().ToScreen();
		_text = _getText();
	}

	protected override int BuildHash() => HashCode.Combine( _text, _position );
}
