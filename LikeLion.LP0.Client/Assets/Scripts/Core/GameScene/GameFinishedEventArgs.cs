using System;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public class GameFinishedEventArgs : EventArgs
    {
        public int WinnerStone;
        public string WinnerGuid;
    }
}
