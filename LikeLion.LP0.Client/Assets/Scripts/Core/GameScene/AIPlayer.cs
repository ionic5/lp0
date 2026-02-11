using System;
using System.Threading;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public class AIPlayer : Player, IPlayer
    {
        private readonly Checkerboard _board;
        private readonly IAIConsole _aiConsole;
        private readonly Core.ILogger _logger;
        private CancellationTokenSource _cts;
        private bool _isDestroyed;

        public event EventHandler<DestroyEventArgs> DestroyEvent;

        public AIPlayer(Checkerboard board, IAIConsole aiConsole, ILogger logger)
        {
            _board = board;
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

            var point = await _aiConsole.RequestStonePoint(GetStoneType(), _board.ToArray(), _cts.Token);
            if (point == null)
                return;

            var column = point.Item1;
            var row = point.Item2;
            _board.TryPutStone(column, row, GetStoneType());
        }

        public void HaltTurn()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
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
