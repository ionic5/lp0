using System;

namespace LikeLion.LP0.Client.Core.View.GameScene
{
    public interface IPickStonePanel : IDestroyable
    {
        event EventHandler WhiteStoneButtonClickedEvent;
        event EventHandler BlackStoneButtonClickedEvent;

        void Hide();
    }
}
