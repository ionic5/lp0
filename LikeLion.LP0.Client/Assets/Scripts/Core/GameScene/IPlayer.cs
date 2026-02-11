namespace LikeLion.LP0.Client.Core.GameScene
{
    public interface IPlayer : IDestroyable
    {
        void SetPlayerGuid(string playerGuid);
        string GetPlayerGuid();
        void StartTurn();
        void HaltTurn();
    }
}
