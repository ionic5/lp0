using LikeLion.LH1.Client.Core;
using System;
using UnityEngine;

namespace LikeLion.LH1.Client.UnityWorld
{
    public class Scene : MonoBehaviour, IDestroyable
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;
        }
    }
}