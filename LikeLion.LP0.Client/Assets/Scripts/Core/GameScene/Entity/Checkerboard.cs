using System;
using System.Collections.Generic;
using System.Linq;

namespace LikeLion.LH1.Client.Core.GameScene.Entity
{
    public class Checkerboard
    {
        private List<List<int>> _board;
        private readonly List<Tuple<string, int>> _stoneOwners;
        private string _gameGuid;

        public Checkerboard()
        {
            _stoneOwners = new List<Tuple<string, int>>();
        }

        public int[][] ToArray()
        {
            return _board.Select(row => row.ToArray()).ToArray();
        }

        public string GetGameGuid()
        {
            return _gameGuid;
        }

        public bool IsStonePointEmpty(int column, int row)
        {
            return _board[column][row] == StoneType.Null;
        }

        public void RegisterStoneOwner(string playerGuid, int stoneType)
        {
            _stoneOwners.Add(new Tuple<string, int>(playerGuid, stoneType));
        }

        public int GetStone(string playerGuid)
        {
            return _stoneOwners.Where(entry => entry.Item1 == playerGuid).Select(entry => entry.Item2).First();
        }

        public void SetGameGuid(string gameGuid)
        {
            _gameGuid = gameGuid;
        }

        public string GetPlayerGuid(int stoneType)
        {
            var playerGuid = _stoneOwners.Where(entry => entry.Item2 == stoneType)
                .Select(entry => entry.Item1).First();
            return playerGuid;
        }

        public string GetOpponentPlayerGuid(string playerGuid)
        {
            return _stoneOwners.Where(entry => entry.Item1 != playerGuid).Select(entry => entry.Item1).First();
        }

        public void PutStone(int column, int row, int stoneType)
        {
            _board[column][row] = stoneType;
        }

        public void Clear()
        {
            _gameGuid = string.Empty;
            _stoneOwners.Clear();

            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                    _board[i][j] = StoneType.Null;
        }

        public void Setup()
        {
            _board = new List<List<int>>();
            for (int i = 0; i < 19; i++)
            {
                List<int> row = new List<int>();
                for (int j = 0; j < 19; j++)
                    row.Add(StoneType.Null);
                _board.Add(row);
            }
        }

        public int CheckWinner()
        {
            return CheckWinner(ToArray());
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
    }
}
