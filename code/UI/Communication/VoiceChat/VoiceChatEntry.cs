using Sandbox;
using Sandbox.UI;
using System;

namespace TTT.UI;

public partial class VoiceChatEntry : Panel
{
	public Friend Friend;

	private readonly WorldPanel _indicator;
	private readonly RolePlate _rolePlate;
	private readonly IClient _client;
	private float _voiceLevel = 0.5f;
	private float _targetVoiceLevel = 0;
	private readonly float _voiceTimeout = 0.1f;

	RealTimeSince _timeSincePlayed;

	public VoiceChatEntry( IClient client )
	{
		_client = client;
		Friend = new( client.SteamId );

		_indicator = new VoiceChatIndicator( _client );
		_rolePlate = client.Pawn.AsEntity().Components.Get<RolePlate>();
	}

	public void Update( float level )
	{
		_timeSincePlayed = 0;
		_targetVoiceLevel = level;
	}

	public override void Tick()
	{
		if ( IsDeleting )
			return;

		var timeoutInv = 1 - (_timeSincePlayed / _voiceTimeout);
		timeoutInv = MathF.Min( timeoutInv * 2.0f, 1.0f );

		if ( timeoutInv <= 0 )
		{
			_indicator?.Delete();
			Delete();
			return;
		}

		_voiceLevel = _voiceLevel.LerpTo( _targetVoiceLevel, Time.Delta * 40.0f );

		if ( !_indicator.IsValid() || _client.Pawn is not Player player || !player.IsAlive )
		{
			_indicator?.Delete();
			return;
		}

		if ( !_indicator.IsEnabled() )
			return;

		var tx = player.GetBoneTransform( "head" );
		var rolePlateOffset = _rolePlate is not null ? 27f : 20f;
		tx.Position += Vector3.Up * rolePlateOffset + (Vector3.Up * _voiceLevel);
		tx.Rotation = Camera.Rotation.RotateAroundAxis( Vector3.Up, 180f );
		_indicator.Transform = tx;
	}

	protected override int BuildHash()
	{
		var player = _client.Pawn as Player;
		return HashCode.Combine( player?.IsAlive, player?.Role.GetHashCode() );
	}
}
