using LikeLion.LP0.Client.Core.View.GameScene;
using UnityEngine;

namespace LikeLion.LP0.Client.UnityWorld.View.GameScene
{
    public class PanelStack : MonoBehaviour, IPanelStack
    {
        [SerializeField]
        private ResultPanel _resultPanel;
        [SerializeField]
        private PickStonePanel _pickStonePanel;

        public IResultPanel ShowResultPanel()
        {
            _resultPanel.gameObject.SetActive(true);
            return _resultPanel;
        }

        public IPickStonePanel ShowPickStonePanel()
        {
            _pickStonePanel.gameObject.SetActive(true);
            return _pickStonePanel;
        }
    }
}
