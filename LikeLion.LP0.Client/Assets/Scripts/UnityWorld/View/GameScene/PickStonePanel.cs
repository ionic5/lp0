using LikeLion.LP0.Client.Core;
using LikeLion.LP0.Client.Core.View.GameScene;
using System;
using UnityEngine;

namespace LikeLion.LP0.Client.UnityWorld.View.GameScene
{
    public class PickStonePanel : MonoBehaviour, IPickStonePanel
    {
        private bool _isDestroyed;

        public event EventHandler WhiteStoneButtonClickedEvent;
        public event EventHandler BlackStoneButtonClickedEvent;
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnWhiteStoneButtonClicked()
        {
            WhiteStoneButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnBlackStoneButtonClicked()
        {
            BlackStoneButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            WhiteStoneButtonClickedEvent = null;
            BlackStoneButtonClickedEvent = null;
        }
    }
}
