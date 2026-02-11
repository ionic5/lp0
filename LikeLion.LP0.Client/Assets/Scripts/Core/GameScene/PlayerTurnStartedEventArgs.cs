using System;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public class PlayerTurnStartedEventArgs : EventArgs
    {
        public string PlayerGuid;
        public float TimeLimit;
    }
}
