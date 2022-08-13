using System;

using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class VoiceChatEntry : Panel
{
	public Friend Friend;

	private Label Name { get; init; }
	private Image Avatar { get; init; }

	private readonly Client _client;
	private float _voiceLevel = 0.5f;
	private float _targetVoiceLevel = 0;
	private readonly float _voiceTimeout = 0.1f;

	RealTimeSince _timeSincePlayed;

	public VoiceChatEntry( Panel parent, Client client ) : base( parent )
	{
		Parent = parent;

		_client = client;
		Friend = new( client.PlayerId );

		Avatar.SetTexture( $"avatar:{client.PlayerId}" );
		Name.Text = Friend.Name;
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
			return;
		}

		_voiceLevel = _voiceLevel.LerpTo( _targetVoiceLevel, Time.Delta * 40.0f );
	}
}
