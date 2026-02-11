using LikeLion.LH1.Client.Core.GameScene;
using UnityEngine;

namespace LikeLion.LH1.Client.UnityWorld.View.GameScene
{
    public class StonePanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject _whiteStone;
        [SerializeField]
        private GameObject _blackStone;

        public void SetStone(int stoneType)
        {
            _whiteStone.SetActive(stoneType == StoneType.White);
            _blackStone.SetActive(stoneType == StoneType.Black);
        }
    }
}
