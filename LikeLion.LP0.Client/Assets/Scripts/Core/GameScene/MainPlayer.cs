using System;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public class MainPlayer : IPlayer
    {
        private readonly ICheckerboard _board;
        private readonly IGameSession _gameSession;
        private bool _isDestroyed;
        private bool _isMyTurn;
        private string _playerGuid;

        public event EventHandler<DestroyEventArgs> DestroyEvent;

        public MainPlayer(ICheckerboard board, IGameSession gameSession)
        {
            _board = board;
            _isDestroyed = false;
            _isMyTurn = false;
            _playerGuid = string.Empty;

            _board.StonePointClickedEvent += OnStonePointClickedEvent;
            _gameSession = gameSession;
        }

        public void StartTurn()
        {
            _isMyTurn = true;
        }

        public void HaltTurn()
        {
            _isMyTurn = false;
        }

        private void OnStonePointClickedEvent(object sender, StonePointClickedEventArgs args)
        {
            if (!_isMyTurn)
                return;

            var column = args.Column;
            var row = args.Row;
            if (_board.IsStonePointEmpty(column, row))
                _gameSession.PutStone(_board.GetGameGuid(), _playerGuid, column, row);
        }

        public void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            _isMyTurn = false;
            _board.StonePointClickedEvent -= OnStonePointClickedEvent;
        }

        public void SetPlayerGuid(string playerGuid)
        {
            _playerGuid = playerGuid;
        }

        public string GetPlayerGuid()
        {
            return _playerGuid;
        }
    }
}
