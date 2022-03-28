using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardEntry : Panel
{
	public string ScoreboardGroupName;
	public Client Client;

	private Image PlayerAvatar { get; set; }
	private Label PlayerName { get; set; }
	private Label Karma { get; set; }
	private Label Ping { get; set; }

	public virtual void Update()
	{
		if ( Client == null )
			return;

		PlayerName.Text = Client.Name;
		Karma.Text = Client.GetInt( "karma" ).ToString();

		SetClass( "me", Client == Local.Client );

		if ( Client.Pawn is not Player player )
			return;

		if ( player.Role is not NoneRole && player.Role is not InnocentRole )
			Style.BackgroundColor = player.Role.Color.WithAlpha( 0.15f );
		else
			Style.BackgroundColor = null;

		PlayerAvatar.SetTexture( $"avatar:{Client.PlayerId}" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Client != null )
			Ping.Text = Client.Ping.ToString();
	}
}
