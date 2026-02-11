using UnityEngine;

namespace LikeLion.LH1.Client.UnityWorld.View.GameScene
{
    public class TurnAlarmPanel : MonoBehaviour
    {
        [SerializeField]
        private StonePanel _stonePanel;

        private float _remainTime;

        public void Play(int stoneType)
        {
            _remainTime = 1.2f;
            _stonePanel.SetStone(stoneType);

            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (_remainTime > 0)
            {
                _remainTime -= UnityEngine.Time.deltaTime;
                if (_remainTime <= 0.0f)
                    _remainTime = 0.0f;

                if (_remainTime == 0.0f)
                    gameObject.SetActive(false);
            }
        }
    }
}
