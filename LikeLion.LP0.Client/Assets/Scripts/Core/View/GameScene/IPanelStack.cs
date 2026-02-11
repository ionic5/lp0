using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LikeLion.LH1.Client.Core.View.GameScene
{
    public interface IPanelStack
    {
        IPickStonePanel ShowPickStonePanel();
        IResultPanel ShowResultPanel();
    }
}
