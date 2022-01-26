using System;

using Sandbox;

using TTT.Player;

namespace TTT.Rounds
{
    public abstract partial class BaseRound : BaseNetworkable
    {
        [Net] public TimeUntil TimeUntilRoundEnds { get; set; }
        public virtual int RoundDuration => 0;
        public virtual string RoundName => "";

        public string TimeLeftFormatted
        {
            get
            {
                return Globals.Utils.TimerString(TimeUntilRoundEnds.Relative);
            }
        }

        public void Start()
        {
            if (Host.IsServer && RoundDuration > 0)
            {
                TimeUntilRoundEnds = RoundDuration;
            }

            OnStart();
        }

        public void Finish()
        {
            if (Host.IsServer)
            {
                TimeUntilRoundEnds = 0f;
            }

            OnFinish();
        }

        public virtual void OnPlayerSpawn(TTTPlayer player)
        {

        }

        public virtual void OnPlayerKilled(TTTPlayer player)
        {

        }

        public virtual void OnPlayerJoin(TTTPlayer player)
        {

        }


        public virtual void OnPlayerLeave(TTTPlayer player)
        {

        }

        public virtual void OnTick()
        {

        }

        public virtual void OnSecond()
        {
            if (Host.IsServer)
            {
                if (TimeUntilRoundEnds)
                {
                    OnTimeUp();
                }
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
