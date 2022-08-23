using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public class VoiceChatEntry : Panel
{
	public Friend Friend;

	private Label Name { get; init; }
	private Image Avatar { get; init; }

	private readonly WorldPanel _worldPanel;
	private readonly Client _client;
	private readonly Player _player;
	private float _voiceLevel = 0.5f;
	private float _targetVoiceLevel = 0;
	private readonly float _voiceTimeout = 0.1f;

	RealTimeSince _timeSincePlayed;

	public VoiceChatEntry( Panel parent, Client client ) : base( parent )
	{
		Parent = parent;

		_client = client;
		_player = client.Pawn as Player;
		Friend = new( client.PlayerId );

		Avatar.SetTexture( $"avatar:{client.PlayerId}" );
		Name.Text = Friend.Name;

		_worldPanel = new WorldPanel();
		_worldPanel.StyleSheet.Load( "/UI/General/VoiceChat/VoiceChatEntry.scss" );
		_worldPanel.Add.Image( classname: "voice-icon" ).SetTexture( "ui/voicechat.png" );
		_worldPanel.SceneObject.Flags.ViewModelLayer = true;
	}

	public void Update( float level )
	{
		_timeSincePlayed = 0;
		_targetVoiceLevel = level;
		Name.Text = Friend.Name;

		if ( _client.IsValid() )
			SetClass( "dead", !_client.Pawn.IsAlive() );
	}

	public override void Tick()
	{
		if ( IsDeleting )
			return;

		var timeoutInv = 1 - (_timeSincePlayed / _voiceTimeout);
		timeoutInv = MathF.Min( timeoutInv * 2.0f, 1.0f );

		if ( timeoutInv <= 0 )
		{
			Delete();
			_worldPanel?.Delete();
			return;
		}

		_voiceLevel = _voiceLevel.LerpTo( _targetVoiceLevel, Time.Delta * 40.0f );

		if ( !_player.IsAlive() )
		{
			_worldPanel?.Delete();
			return;
		}

		var tx = _player.GetBoneTransform( "head" );
		tx.Position += Vector3.Up * 18f + (Vector3.Up * _voiceLevel);
		tx.Rotation = CurrentView.Rotation.RotateAroundAxis( Vector3.Up, 180f );
		_worldPanel.Transform = tx;
	}
}
