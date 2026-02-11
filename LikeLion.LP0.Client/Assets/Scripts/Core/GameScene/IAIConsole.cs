using System;
using System.Threading.Tasks;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public interface IAIConsole
    {
        Task<Tuple<int, int>> RequestStonePoint(int stoneType, int[][] array, System.Threading.CancellationToken token);
    }
}
