using System;

namespace LikeLion.LH1.Client.Core.View.Scenes
{
    public interface ITitleScene : IDestroyable
    {
        event EventHandler OmokButtonClickedEvent;
    }
}
