using System;
using LikeLion.LH1.Client.Core.View.GameScene;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public class PanelHandler
    {
        private readonly IPanelStack _panelStack;
        private readonly ICheckerboard _board;
        private readonly IPlayer _player;
        private readonly IGameSession _session;
        private readonly ILogger _logger;
        private readonly Action _loadTitleScene;

        private GameHost _host;

        public PanelHandler(IPanelStack panelStack, ICheckerboard board, IPlayer player,
                            IGameSession session, ILogger logger, Action loadTitleScene)
        {
            _panelStack = panelStack;
            _board = board;
            _player = player;
            _session = session;
            _logger = logger;
            _loadTitleScene = loadTitleScene;
        }

        public void SetHost(GameHost host)
        {
            _host = host;
        }

        public void ShowPickStonePanel()
        {
            _logger.Info("Show pick stone panel.");
            var panel = _panelStack.ShowPickStonePanel();
            var ctrl = new PickStonePanelController(_board, _player, panel, _session, _host.Start, _logger);
        }

        public void ShowResultPanel(bool isWinner)
        {
            _logger.Info("Show result panel.");
            var panel = _panelStack.ShowResultPanel();
            panel.SetResult(isWinner);
            var ctlr = new ResultPanelController(_host, panel, _loadTitleScene);
        }

        public void Destroy()
        {
            _host = null;
        }
    }
}
