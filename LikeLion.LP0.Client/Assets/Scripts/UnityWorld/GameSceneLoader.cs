using LikeLion.LH1.Client.Core;
using LikeLion.LH1.Client.Core.GameScene;
using LikeLion.LH1.Client.UnityWorld.GameScene;
using System;

namespace LikeLion.LH1.Client.UnityWorld
{
    public class GameSceneLoader
    {
        private readonly Screen _screen;
        private readonly Core.ILogger _logger;
        private readonly ITime _time;
        private readonly Action _loadTitleScene;

        public GameSceneLoader(Screen screen, ILogger logger, ITime time, Action loadTitleScene)
        {
            _screen = screen;
            _logger = logger;
            _time = time;
            _loadTitleScene = loadTitleScene;
        }

        public async void Load()
        {
            await _screen.ShowLoadingBlind();
            _screen.DestroyLastScene();

            var instance = await _screen.AttachNewScene("Assets/Addressables/GameScene.prefab");
            var scene = instance.GetComponent<View.Scenes.GameScene>();

            var checkerBoard = scene.CheckerBoard;
            var loop = scene.Loop;
            var mainUIPanel = scene.MainUIPanel;
            var panelStack = scene.PanelStack;

            IGameSession gameSession = null;
            var mockCheckeboard = new Core.GameScene.Entity.Checkerboard();
            gameSession = new MockGameSession(new Core.Timer(_time, loop),
                mockCheckeboard,
                () =>
                {
                    return new AIPlayer(mockCheckeboard,
                    gameSession, new AIConsole(_logger), _logger);
                });
            var boardEntity = new Core.GameScene.Entity.Checkerboard();
            boardEntity.Setup();
            var board = new Core.GameScene.Checkerboard(checkerBoard, boardEntity, _logger);
            var player = new MainPlayer(board, gameSession);

            var panelHdlr = new PanelHandler(panelStack, board, player, gameSession, _logger, _loadTitleScene);
            var host = new GameHost(gameSession, board, player, mainUIPanel, new Core.Timer(_time, loop), panelHdlr);

            panelHdlr.SetHost(host);

            host.Connect();
            loop.Add(host);

            scene.DestroyEvent += (sender, args) =>
            {
                host.Destroy();
                loop.Remove(host);
            };

            _screen.HideLoadingBlind();
        }
    }
}