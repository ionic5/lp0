namespace LikeLion.LP0.Client.Core.GameScene
{
    public interface IPlayer : IDestroyable
    {
        int GetStoneType();
        void HaltTurn();
        bool IsStoneOwner(int stoneType);
        void SetStone(int white);
        void StartTurn();
    }
}
