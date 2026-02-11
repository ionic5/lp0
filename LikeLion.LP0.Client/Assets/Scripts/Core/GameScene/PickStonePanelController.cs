using LikeLion.LP0.Client.Core.View.GameScene;
using System;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public class PickStonePanelController
    {
        private readonly IPlayer _opponentPlayer;
        private readonly IPlayer _mainPlayer;
        private readonly GameHost _omokHost;
        private readonly IPickStonePanel _pickStonePanel;

        public PickStonePanelController(IPlayer mainPlayer, IPlayer opponentPlayer, GameHost omokHost, IPickStonePanel pickStonePanel)
        {
            _mainPlayer = mainPlayer;
            _opponentPlayer = opponentPlayer;
            _omokHost = omokHost;
            _pickStonePanel = pickStonePanel;

            _pickStonePanel.BlackStoneButtonClickedEvent += OnBlackStoneButtonClickedEvent;
            _pickStonePanel.WhiteStoneButtonClickedEvent += OnWhiteStoneButtonClickedEvent;
            _pickStonePanel.DestroyEvent += OnDestroyPanelEvent;
        }

        private void OnDestroyPanelEvent(object sender, DestroyEventArgs e)
        {
            DetachEventHandlers();
        }

        private void DetachEventHandlers()
        {
            _pickStonePanel.BlackStoneButtonClickedEvent -= OnBlackStoneButtonClickedEvent;
            _pickStonePanel.WhiteStoneButtonClickedEvent -= OnWhiteStoneButtonClickedEvent;
            _pickStonePanel.DestroyEvent -= OnDestroyPanelEvent;
        }

        public void OnWhiteStoneButtonClickedEvent(object sender, EventArgs args)
        {
            AssignStones(StoneType.White);
        }

        public void OnBlackStoneButtonClickedEvent(object sender, EventArgs args)
        {
            AssignStones(StoneType.Black);
        }

        private void AssignStones(int mainPlayerStone)
        {
            _mainPlayer.SetStone(mainPlayerStone);
            _opponentPlayer.SetStone(mainPlayerStone == StoneType.White ? StoneType.Black : StoneType.White);

            DetachEventHandlers();

            _pickStonePanel.Hide();

            _omokHost.Start();
        }
    }
}
