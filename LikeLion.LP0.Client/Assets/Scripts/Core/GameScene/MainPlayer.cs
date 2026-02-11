using System;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public class MainPlayer : Player, IPlayer
    {
        private readonly Checkerboard _board;
        private bool _isDestroyed;
        private bool _isMyTurn;

        public event EventHandler<DestroyEventArgs> DestroyEvent;

        public MainPlayer(Checkerboard board)
        {
            _board = board;
            _isDestroyed = false;
            _isMyTurn = false;
        }

        public void StartTurn()
        {
            _isMyTurn = true;
            _board.StonePointClickedEvent += OnStonePointClickedEvent;
        }

        public void HaltTurn()
        {
            _isMyTurn = false;
            _board.StonePointClickedEvent -= OnStonePointClickedEvent;
        }

        private void OnStonePointClickedEvent(object sender, StonePointClickedEventArgs args)
        {
            if (!_isMyTurn)
                return;

            _board.TryPutStone(args.Column, args.Row, GetStoneType());
        }

        public void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            if (_isMyTurn)
                _board.StonePointClickedEvent -= OnStonePointClickedEvent;
            _isMyTurn = false;
        }
    }
}
