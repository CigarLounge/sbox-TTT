using System;

using Sandbox;

using TTT.Player;

namespace TTT.Rounds
{
	public abstract partial class BaseRound : BaseNetworkable
	{
		[Net]
		public TimeUntil TimeUntilRoundEnd { get; set; }

		public virtual int RoundDuration => 0;
		public virtual string RoundName => "";

		public string TimeLeftFormatted { get { return Utils.TimerString( TimeUntilRoundEnd.Relative ); } }

		public void Start()
		{
			if ( Host.IsServer && RoundDuration > 0 )
			{
				TimeUntilRoundEnd = RoundDuration;
			}

			OnStart();
		}

		public void Finish()
		{
			if ( Host.IsServer )
			{
				TimeUntilRoundEnd = 0f;
			}

			OnFinish();
		}

		public virtual void OnPlayerSpawn( TTTPlayer player )
		{

		}

		public virtual void OnPlayerKilled( TTTPlayer player )
		{

		}

		public virtual void OnPlayerJoin( TTTPlayer player )
		{

		}


		public virtual void OnPlayerLeave( TTTPlayer player )
		{

		}

		public virtual void OnTick()
		{

		}

		public virtual void OnSecond()
		{
			if ( Host.IsServer && TimeUntilRoundEnd )
			{
				OnTimeUp();
			}
		}

		protected virtual void OnStart()
		{

		}

		protected virtual void OnFinish()
		{

		}

		protected virtual void OnTimeUp()
		{

		}
	}
}
