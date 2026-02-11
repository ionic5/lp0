using LikeLion.LH1.Client.Core.TitleScene;
using LikeLion.LH1.Client.UnityWorld.View.Scenes;
using System;

namespace LikeLion.LH1.Client.UnityWorld
{
    public class TitleSceneLoader
    {
        private readonly Screen _screen;
        private readonly Action _loadGameScene;

        public TitleSceneLoader(Screen screen, Action loadGameScene)
        {
            _screen = screen;
            _loadGameScene = loadGameScene;
        }

        public async void Load()
        {
            await _screen.ShowLoadingBlind();
            _screen.DestroyLastScene();

            var instance = await _screen.AttachNewScene("Assets/Addressables/TitleScene.prefab");
            var scene = instance.GetComponent<TitleScene>();
            var ctrl = new TitleSceneController(scene, _loadGameScene);

            _screen.HideLoadingBlind();
        }
    }
}
