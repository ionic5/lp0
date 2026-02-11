using System;
using System.Threading;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public class AIPlayer : IPlayer
    {
        private readonly Entity.Checkerboard _board;
        private readonly IAIConsole _aiConsole;
        private readonly Core.ILogger _logger;
        private readonly IGameSession _gameSession;
        private CancellationTokenSource _cts;
        private bool _isDestroyed;
        private string _playerGuid;

        public event EventHandler<DestroyEventArgs> DestroyEvent;

        public AIPlayer(Entity.Checkerboard board, IGameSession gameSession, IAIConsole aiConsole, ILogger logger)
        {
            _board = board;
            _gameSession = gameSession;
            _aiConsole = aiConsole;
            _cts = null;
            _isDestroyed = false;
            _logger = logger;
        }

        public async void StartTurn()
        {
            if (_cts != null)
            {
                _logger.Warn("StartTurn() was called while a turn is already in progress. (Current _cts is not null)");
                return;
            }

            _cts = new CancellationTokenSource();

            var point = await _aiConsole.RequestStonePoint(_board.GetStone(_playerGuid), _board.ToArray(), _cts.Token);
            if (point == null)
                return;

            var column = point.Item1;
            var row = point.Item2;
            if (_board.IsStonePointEmpty(column, row))
                _gameSession.PutStone(_board.GetGameGuid(), _playerGuid, column, row);
        }

        public void HaltTurn()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void SetPlayerGuid(string playerGuid)
        {
            _playerGuid = playerGuid;
        }

        public string GetPlayerGuid()
        {
            return _playerGuid;
        }

        public void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            if (_cts != null)
                HaltTurn();
        }
    }
}
