using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace TTT.UI;

public partial class ScoreboardEntry : Panel
{
	public PlayerStatus PlayerStatus;
	public IClient Client { get; set; }

	private static readonly ColorGroup[] _tagGroups = new ColorGroup[]
	{
		new ColorGroup("Friend", Color.FromBytes(0, 255, 0)),
		new ColorGroup("Suspect", Color.FromBytes(179, 179, 20)),
		new ColorGroup("Missing", Color.FromBytes(130, 190, 130)),
		new ColorGroup("Kill", Color.FromBytes(255, 0, 0))
	};

	private Label Tag { get; set; }
	private Panel DropdownPanel { get; set; }

	public void OnClick()
	{
		if ( DropdownPanel is not null )
		{
			DropdownPanel.Delete();
			DropdownPanel = null;
			return;
		}

		DropdownPanel = new Panel( this, "dropdown" );
		foreach ( var tagGroup in _tagGroups )
		{
			var tagButton = DropdownPanel.Add.Button( tagGroup.Title, () => { SetTag( tagGroup ); } );
			tagButton.Style.FontColor = tagGroup.Color;
		}
	}

	protected override int BuildHash()
	{
		var player = Client.Pawn as Player;
		return !player.IsValid() ? -1 :
				HashCode.Combine(
					player.BaseKarma,
					player.Score,
					player.Role,
					Client.IsBot,
					Client.Ping
				);
	}

	private void SetTag( ColorGroup tagGroup )
	{
		if ( tagGroup.Title == Tag.Text )
		{
			ResetTag();
			return;
		}

		Tag.Text = tagGroup.Title;
		Tag.Style.FontColor = tagGroup.Color;
		(Client.Pawn as Player).TagGroup = tagGroup;
	}

	private void ResetTag()
	{
		Tag.Text = string.Empty;

		if ( Client.IsValid() && Client.Pawn.IsValid() )
			(Client.Pawn as Player).TagGroup = default;
	}

	[Event.Entity.PostCleanup]
	private void OnRoundStart()
	{
		ResetTag();
	}
}
