using LikeLion.LP0.Client.Core.View.GameScene;
using System;

namespace LikeLion.LP0.Client.Core.GameScene
{
    public class GameHost : IUpdatable
    {
        private readonly IGameSession _gameSession;
        private readonly IPlayer _mainPlayer;
        private readonly ICheckerboard _checkerboard;
        private readonly IMainUIPanel _mainUIPanel;
        private readonly Core.Timer _timer;
        private readonly PanelHandler _panelHandler;

        private IGameState _currentState;

        private interface IGameState
        {
            void Enter();
            void Exit();
            void Update();
        }

        private class ConnectingState : IGameState
        {
            private readonly GameHost _host;

            public ConnectingState(GameHost host)
            {
                _host = host;
            }

            public void Enter()
            {
                _host._gameSession.ConnectedEvent += OnConnected;
                _host._gameSession.RequestConnect();
            }

            private void OnConnected(object sender, ConnectedEventArgs args)
            {
                _host._mainPlayer.SetPlayerGuid(args.PlayerGuid);
                _host.ChangeState(new WaitingState(_host));
            }

            public void Exit()
            {
                _host._gameSession.ConnectedEvent -= OnConnected;
            }

            public void Update() { }
        }

        private class WaitingState : IGameState
        {
            private readonly GameHost _host;

            public WaitingState(GameHost host)
            {
                _host = host;
            }

            public void Enter()
            {
                _host._gameSession.GameCreatedEvent += OnGameCreated;
                _host._gameSession.RequestGame(_host._mainPlayer.GetPlayerGuid());
            }

            private void OnGameCreated(object s, GameCreatedEventArgs e)
            {
                _host._checkerboard.SetGameGuid(e.GameGuid);
                _host._panelHandler.ShowPickStonePanel();
            }

            public void Exit()
            {
                _host._gameSession.GameCreatedEvent -= OnGameCreated;
            }

            public void Update() { }
        }

        private class PlayingState : IGameState
        {
            private readonly GameHost _host;

            public PlayingState(GameHost host)
            {
                _host = host;
            }

            public void Enter()
            {
                _host._gameSession.PlayerTurnStartedEvent += OnTurnStarted;
                _host._gameSession.PlayerTurnFinishedEvent += OnTurnFinished;
                _host._gameSession.GameFinishedEvent += OnGameFinished;

                _host._gameSession.StartGame(_host._checkerboard.GetGameGuid(), _host._mainPlayer.GetPlayerGuid());

                _host._mainUIPanel.Show();
                _host._mainUIPanel.SetMainPlayerStone(_host._checkerboard.GetStone(_host._mainPlayer.GetPlayerGuid()));
            }

            private void OnTurnStarted(object sender, PlayerTurnStartedEventArgs args)
            {
                var stone = _host._checkerboard.GetStone(args.PlayerGuid);

                _host._mainUIPanel.PlayTurnStartAnimation(stone);
                _host._mainUIPanel.SetCurrentPlayerStone(stone);
                _host._timer.Start(0, args.TimeLimit);

                if (_host._mainPlayer.GetPlayerGuid() == args.PlayerGuid)
                    _host._mainPlayer.StartTurn();
            }

            private void OnTurnFinished(object sender, PlayerTurnFinishedEventArgs args)
            {
                _host._timer.Stop(0);

                if (args.StoneType != StoneType.Null)
                    _host._checkerboard.PutStone(args.Column, args.Row, args.StoneType);

                if (_host._mainPlayer.GetPlayerGuid() == args.PlayerGuid)
                    _host._mainPlayer.HaltTurn();
            }

            private void OnGameFinished(object sender, GameFinishedEventArgs args)
            {
                _host._timer.Stop(0);
                _host._mainUIPanel.Hide();
                bool isWinner = _host._mainPlayer.GetPlayerGuid() == args.WinnerGuid;
                _host._panelHandler.ShowResultPanel(isWinner);
            }

            public void Exit()
            {
                _host._gameSession.PlayerTurnStartedEvent -= OnTurnStarted;
                _host._gameSession.PlayerTurnFinishedEvent -= OnTurnFinished;
                _host._gameSession.GameFinishedEvent -= OnGameFinished;
            }

            public void Update()
            {
                if (_host._timer.IsRunning(0))
                    _host._mainUIPanel.SetRemainTime(_host._timer.GetRemainTime(0));
            }
        }

        public GameHost(IGameSession gameSession, ICheckerboard checkerboard, IPlayer mainPlayer,
            IMainUIPanel mainUIPanel, Timer timer,
            PanelHandler panelHandler)
        {
            _gameSession = gameSession;
            _mainPlayer = mainPlayer;
            _checkerboard = checkerboard;
            _panelHandler = panelHandler;
            _mainUIPanel = mainUIPanel;
            _timer = timer;
        }

        private void ChangeState(IGameState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void Connect()
        {
            ChangeState(new ConnectingState(this));
        }

        public void Start()
        {
            ChangeState(new PlayingState(this));
        }

        public void Restart()
        {
            _checkerboard.Clear();
            ChangeState(new WaitingState(this));
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void Destroy()
        {
            _currentState?.Exit();
            _currentState = null;

            _timer.Destroy();
            _mainPlayer.Destroy();
            _gameSession.Destroy();
            _checkerboard.Destroy();
            _panelHandler.Destroy();
        }
    }
}
