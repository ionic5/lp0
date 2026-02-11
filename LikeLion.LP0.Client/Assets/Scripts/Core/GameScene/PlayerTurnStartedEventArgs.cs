using System;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public class PlayerTurnStartedEventArgs : EventArgs
    {
        public string PlayerGuid;
        public float TimeLimit;
    }
}
