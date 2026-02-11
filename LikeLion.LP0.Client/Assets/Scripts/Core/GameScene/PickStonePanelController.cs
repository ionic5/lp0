using LikeLion.LH1.Client.Core.View.GameScene;
using System;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public class PickStonePanelController
    {
        private readonly IPickStonePanel _pickStonePanel;
        private readonly IGameSession _gameSession;
        private readonly IPlayer _player;
        private readonly Action _startGame;
        private readonly Core.ILogger _logger;
        private readonly ICheckerboard _checkerboard;

        public PickStonePanelController(ICheckerboard checkerboard, IPlayer player,
            IPickStonePanel pickStonePanel, IGameSession gameSession, Action startGame, ILogger logger)
        {
            _checkerboard = checkerboard;
            _gameSession = gameSession;
            _player = player;
            _pickStonePanel = pickStonePanel;
            _startGame = startGame;
            _logger = logger;

            _logger.Info("Event handler attached.");
            _pickStonePanel.BlackStoneButtonClickedEvent += OnBlackStoneButtonClickedEvent;
            _pickStonePanel.WhiteStoneButtonClickedEvent += OnWhiteStoneButtonClickedEvent;
            _pickStonePanel.DestroyEvent += OnDestroyPanelEvent;
            _gameSession.GamePreparedEvent += OnGamePreparedEvent;
        }

        private void OnGamePreparedEvent(object sender, GamePreparedEventArgs args)
        {
            DetachEventHandlers();
            _pickStonePanel.Hide();

            foreach (var entry in args.StoneOwners)
                _checkerboard.RegisterStoneOwner(entry.PlayerGuid, entry.StoneType);

            _startGame.Invoke();
        }

        private void OnDestroyPanelEvent(object sender, DestroyEventArgs e)
        {
            DetachEventHandlers();
        }

        private void DetachEventHandlers()
        {
            _logger.Info("Event handler detached.");

            _pickStonePanel.BlackStoneButtonClickedEvent -= OnBlackStoneButtonClickedEvent;
            _pickStonePanel.WhiteStoneButtonClickedEvent -= OnWhiteStoneButtonClickedEvent;
            _pickStonePanel.DestroyEvent -= OnDestroyPanelEvent;
            _gameSession.GamePreparedEvent -= OnGamePreparedEvent;
        }

        public void OnWhiteStoneButtonClickedEvent(object sender, EventArgs args)
        {
            _logger.Info("White stone button clicked.");

            PickStone(StoneType.White);
        }

        public void OnBlackStoneButtonClickedEvent(object sender, EventArgs args)
        {
            _logger.Info("Black stone button clicked.");

            PickStone(StoneType.Black);
        }

        private void PickStone(int stoneType)
        {
            _gameSession.PickStone(_checkerboard.GetGameGuid(), _player.GetPlayerGuid(), stoneType);
        }
    }
}
