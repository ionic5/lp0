using System;

namespace LikeLion.LP0.Client.Core
{
    public interface IDestroyable
    {
        event EventHandler<DestroyEventArgs> DestroyEvent;
        void Destroy();
    }
}
