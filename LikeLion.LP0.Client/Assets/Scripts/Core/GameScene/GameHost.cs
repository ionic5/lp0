using LikeLion.LP0.Client.Core.View.GameScene;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public class GameHost : IUpdatable
    {
        public event EventHandler StartGameEvent;
        public event EventHandler<GameFinishedEventArgs> GameFinishedEvent;

        private readonly Checkerboard _checkerboard;
        private readonly List<IPlayer> _players;
        private readonly Core.Timer _timer;
        private readonly float _limitTime;
        private readonly IMainUIPanel _mainUIPanel;

        public GameHost(Checkerboard checkerboard, List<IPlayer> players, Timer timer, float limitTime, IMainUIPanel mainUIPanel)
        {
            _checkerboard = checkerboard;
            _players = players;
            _timer = timer;
            _limitTime = limitTime;
            _checkerboard.StonePuttedEvent += OnStonePuttedEvent;
            _mainUIPanel = mainUIPanel;
        }

        public void Reset()
        {
            _checkerboard.Clear();
        }

        public void Start()
        {
            StartGameEvent?.Invoke(this, EventArgs.Empty);

            var player = _players.First(entry => entry.IsStoneOwner(StoneType.Black));
            StartTurn(player);
        }

        private void OnStonePuttedEvent(object sender, StonePuttedEventArgs args)
        {
            var player = _players.First(entry => entry.IsStoneOwner(args.StoneType));
            player.HaltTurn();

            var winnerStone = CheckWinner(_checkerboard.ToArray());
            if (winnerStone == StoneType.Null)
            {
                var otherPlayer = _players.First(entry => !entry.IsStoneOwner(args.StoneType));
                StartTurn(otherPlayer);
                return;
            }

            _timer.Stop(0);
            GameFinishedEvent?.Invoke(this, new GameFinishedEventArgs { WinnerStone = winnerStone });
        }

        private void StartTurn(IPlayer player)
        {
            _mainUIPanel.PlayTurnStartAnimation(player.GetStoneType());
            _mainUIPanel.SetCurrentPlayerStone(player.GetStoneType());

            player.StartTurn();

            _timer.Stop(0);
            _timer.Start(0, _limitTime, () =>
            {
                player.HaltTurn();

                var otherPlayer = _players.First(entry => entry != player);
                StartTurn(otherPlayer);
            });
        }

        private int CheckWinner(int[][] board)
        {
            int rows = board.Length;
            if (rows == 0) return StoneType.Null;
            int cols = board[0].Length;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (board[r][c] == 0) continue;

                    int stone = board[r][c];

                    if (CheckDirection(board, r, c, 0, 1, stone) || // Horizontal
                        CheckDirection(board, r, c, 1, 0, stone) || // Vertical
                        CheckDirection(board, r, c, 1, 1, stone) || // Right down
                        CheckDirection(board, r, c, 1, -1, stone))  // Left down
                    {
                        return stone;
                    }
                }
            }

            return StoneType.Null;
        }

        private bool CheckDirection(int[][] board, int r, int c, int dr, int dc, int stone)
        {
            int count = 1;
            int rows = board.Length;
            int cols = board[0].Length;

            for (int i = 1; i < 5; i++)
            {
                int nr = r + (dr * i);
                int nc = c + (dc * i);

                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && board[nr][nc] == stone)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count == 5;
        }

        public void Update()
        {
            if (_timer.IsRunning(0))
                _mainUIPanel.SetRemainTime(_timer.GetRemainTime(0));
        }

        public void Destroy()
        {
            StartGameEvent = null;
            GameFinishedEvent = null;
            _checkerboard.StonePuttedEvent -= OnStonePuttedEvent;
        }
    }
}
