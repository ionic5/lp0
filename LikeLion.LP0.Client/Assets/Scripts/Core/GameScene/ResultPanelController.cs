using LikeLion.LP0.Client.Core.View.GameScene;
using System;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public class ResultPanelController
    {
        private readonly GameHost _omokHost;
        private readonly IResultPanel _panel;
        private readonly Action _showPickStonePanel;
        private readonly Action _loadTitleScene;

        public ResultPanelController(GameHost omokHost, IResultPanel panel,
            Action showPickStonePanel, Action loadTitleScene)
        {
            _omokHost = omokHost;
            _panel = panel;

            _panel.RestartButtonClickedEvent += OnRestartButtonClickedEvent;
            _panel.ToTitleButtonClickedEvent += OnToTitleButtonClickedEvent;
            _panel.DestroyEvent += OnDestroyPanelEvent;

            _showPickStonePanel = showPickStonePanel;
            _loadTitleScene = loadTitleScene;
        }

        private void OnDestroyPanelEvent(object sender, DestroyEventArgs e)
        {
            DetachEventHandlers();
        }

        private void DetachEventHandlers()
        {
            _panel.RestartButtonClickedEvent -= OnRestartButtonClickedEvent;
            _panel.ToTitleButtonClickedEvent -= OnToTitleButtonClickedEvent;
            _panel.DestroyEvent -= OnDestroyPanelEvent;
        }

        private void OnToTitleButtonClickedEvent(object sender, EventArgs e)
        {
            HidePanel();

            _loadTitleScene.Invoke();
        }

        public void OnRestartButtonClickedEvent(object sender, EventArgs args)
        {
            HidePanel();

            _omokHost.Reset();

            _showPickStonePanel.Invoke();
        }

        private void HidePanel()
        {
            DetachEventHandlers();

            _panel.Hide();
        }
    }
}
