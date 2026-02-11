using System;

namespace LikeLion.LP0.Client.Core.View.Scenes
{
    public interface ITitleScene : IDestroyable
    {
        event EventHandler OmokButtonClickedEvent;
    }
}
