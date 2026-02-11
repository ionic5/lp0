using LikeLion.LP0.Client.Core;
using LikeLion.LP0.Client.Core.View.GameScene;
using System;
using UnityEngine;

namespace LikeLion.LP0.Client.UnityWorld.View.GameScene
{
    public class ResultPanel : MonoBehaviour, IResultPanel
    {
        [SerializeField]
        private GameObject _winPanel;
        [SerializeField]
        private GameObject _losePanel;
        private bool _isDestroyed;

        public event EventHandler RestartButtonClickedEvent;
        public event EventHandler ToTitleButtonClickedEvent;
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        public void SetResult(bool isWin)
        {
            _winPanel.SetActive(isWin);
            _losePanel.SetActive(!isWin);
        }

        public void OnRestartButtonClicked()
        {
            RestartButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnToTitleButtonClicked()
        {
            ToTitleButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
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

            RestartButtonClickedEvent = null;
            ToTitleButtonClickedEvent = null;
        }
    }
}
