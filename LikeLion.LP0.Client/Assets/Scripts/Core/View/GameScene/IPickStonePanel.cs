using System;

namespace LikeLion.LH1.Client.Core.View.GameScene
{
    public interface IPickStonePanel : IDestroyable
    {
        event EventHandler WhiteStoneButtonClickedEvent;
        event EventHandler BlackStoneButtonClickedEvent;

        void Hide();
    }
}
