namespace LikeLion.LH1.Client.Core.View.GameScene
{
    public interface IMainUIPanel
    {
        void Show();
        void SetMainPlayerStone(int stoneType);
        void PlayTurnStartAnimation(int stoneType);
        void SetRemainTime(float remainTime);
        void SetCurrentPlayerStone(int stoneType);
        void Hide();
    }
}
