using System;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public class Checkerboard : ICheckerboard
    {
        private readonly View.GameScene.ICheckerboard _checkerboardView;
        private readonly Core.ILogger _logger;
        private readonly Entity.Checkerboard _board;
        private bool _isDestroyed;

        public event EventHandler<StonePointClickedEventArgs> StonePointClickedEvent;
        public event EventHandler<StonePuttedEventArgs> StonePuttedEvent;

        public Checkerboard(View.GameScene.ICheckerboard checkerboardView, Entity.Checkerboard board, ILogger logger)
        {
            _checkerboardView = checkerboardView;
            _checkerboardView.StonePointClickedEvent += OnStonePointClickedEvent;
            _checkerboardView.DestroyEvent += OnDestroyViewEvent;

            _logger = logger;
            _board = board;
        }

        private void OnDestroyViewEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        public void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _checkerboardView.StonePointClickedEvent -= OnStonePointClickedEvent;
            _checkerboardView.DestroyEvent -= OnDestroyViewEvent;
            _checkerboardView.Destroy();

            _board.Clear();
        }

        private void OnStonePointClickedEvent(object sender, View.GameScene.StonePointClickedEventArgs args)
        {
            StonePointClickedEvent?.Invoke(this, new StonePointClickedEventArgs
            {
                Row = args.Row,
                Column = args.Column
            });
        }

        public int[][] ToArray()
        {
            return _board.ToArray();
        }

        public void PutStone(int column, int row, int stoneType)
        {
            if (!IsStonePointEmpty(column, row))
            {
                _logger.Warn("Attempted to place a stone on a non-empty point. Ignored.");
                return;
            }

            _board.PutStone(column, row, stoneType);
            _checkerboardView.PutStone(column, row, stoneType);

            StonePuttedEvent?.Invoke(this, new StonePuttedEventArgs { StoneType = stoneType });
            return;
        }

        public void Clear()
        {
            _board.Clear();
            _checkerboardView.Clear();
        }

        public bool IsStonePointEmpty(int column, int row)
        {
            return _board.IsStonePointEmpty(column, row);
        }

        public string GetGameGuid()
        {
            return _board.GetGameGuid();
        }

        public void SetGameGuid(string gameGuid)
        {
            _board.SetGameGuid(gameGuid);
        }

        public void RegisterStoneOwner(string playerGuid, int stoneType)
        {
            _board.RegisterStoneOwner(playerGuid, stoneType);
        }

        public int GetStone(string playerGuid)
        {
            return _board.GetStone(playerGuid);
        }
    }
}
