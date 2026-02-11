using System;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public interface ICheckerboard
    {
        event EventHandler<StonePointClickedEventArgs> StonePointClickedEvent;

        void SetGameGuid(string gameGuid);
        string GetGameGuid();
        bool IsStonePointEmpty(int column, int row);
        void PutStone(int column, int row, int stoneType);
        void RegisterStoneOwner(string playerGuid, int stoneType);
        int GetStone(string playerGuid);
        void Clear();
        void Destroy();
    }
}
