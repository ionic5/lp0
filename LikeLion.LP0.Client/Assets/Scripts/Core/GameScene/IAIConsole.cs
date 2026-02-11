using System;
using System.Threading.Tasks;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public interface IAIConsole
    {
        Task<Tuple<int, int>> RequestStonePoint(int stoneType, int[][] array, System.Threading.CancellationToken token);
    }
}
