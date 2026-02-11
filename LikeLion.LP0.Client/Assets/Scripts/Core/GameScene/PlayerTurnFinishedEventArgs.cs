using System;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public class PlayerTurnFinishedEventArgs : EventArgs
    {
        public string PlayerGuid;
        public int StoneType;
        public int Column;
        public int Row;
    }
}
