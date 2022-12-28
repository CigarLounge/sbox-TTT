using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace TTT.UI;

public partial class ScoreboardEntry : Panel
{
	public Player Player { get; set; }

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
		return HashCode.Combine(
			Player.Role,
			Player.SteamId,
			Player.BaseKarma,
			Player.Score,
			Player.Client.Ping
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
		Player.TagGroup = tagGroup;
	}

	private void ResetTag()
	{
		if ( Tag != null )
			Tag.Text = string.Empty;

		if ( Player.IsValid() )
			Player.TagGroup = default;
	}

	[Event.Entity.PostCleanup]
	private void OnRoundStart()
	{
		ResetTag();
	}
}
