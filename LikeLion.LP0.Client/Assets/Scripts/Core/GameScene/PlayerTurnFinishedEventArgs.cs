using System;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public class PlayerTurnFinishedEventArgs : EventArgs
    {
        public string PlayerGuid;
        public int StoneType;
        public int Column;
        public int Row;
    }
}
