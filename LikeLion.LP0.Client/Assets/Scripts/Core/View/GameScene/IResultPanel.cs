using System;

namespace LikeLion.LP0.Client.Core.View.GameScene
{
    public interface IResultPanel : IDestroyable
    {
        event EventHandler RestartButtonClickedEvent;
        event EventHandler ToTitleButtonClickedEvent;

        void Hide();
        void SetResult(bool v);
    }
}
