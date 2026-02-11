using System;

namespace LikeLion.LH1.Client.Core
{
    public interface IDestroyable
    {
        event EventHandler<DestroyEventArgs> DestroyEvent;
        void Destroy();
    }
}
