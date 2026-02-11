namespace LikeLion.LP0.Client.Core.View.GameScene
{
    public interface IMainUIPanel
    {
        void SetMainPlayerStone(int stoneType);
        void PlayTurnStartAnimation(int stoneType);
        void SetRemainTime(float remainTime);
        void SetCurrentPlayerStone(int stoneType);
    }
}
