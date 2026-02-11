using LikeLion.LP0.Client.Core.View.Scenes;
using System;

namespace LikeLion.LP0.Client.Core.TitleScene
{
    public class TitleSceneController
    {
        private readonly ITitleScene _scene;
        private readonly Action _loadGameScene;

        public TitleSceneController(ITitleScene scene, Action loadGameScene)
        {
            _scene = scene;
            _loadGameScene = loadGameScene;

            _scene.OmokButtonClickedEvent += OnOmokButtonClickedEvent;
            _scene.DestroyEvent += OnDestroySceneEvent;
        }

        private void OnOmokButtonClickedEvent(object sender, EventArgs args)
        {
            _loadGameScene.Invoke();
        }

        private void OnDestroySceneEvent(object sender, EventArgs args)
        {
            _scene.OmokButtonClickedEvent -= OnOmokButtonClickedEvent;
            _scene.DestroyEvent -= OnDestroySceneEvent;
        }
    }
}
