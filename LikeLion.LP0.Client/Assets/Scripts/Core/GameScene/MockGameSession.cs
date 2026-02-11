using System;
using System.Collections.Generic;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public class MockGameSession : IGameSession
    {
        public event EventHandler<ConnectedEventArgs> ConnectedEvent;
        public event EventHandler<GameCreatedEventArgs> GameCreatedEvent;
        public event EventHandler<GamePreparedEventArgs> GamePreparedEvent;
        public event EventHandler<PlayerTurnStartedEventArgs> PlayerTurnStartedEvent;
        public event EventHandler<PlayerTurnFinishedEventArgs> PlayerTurnFinishedEvent;
        public event EventHandler<GameFinishedEventArgs> GameFinishedEvent;

        private readonly Core.Timer _timer;
        private readonly float _timeLimit;
        private readonly Entity.Checkerboard _checkerboard;
        private readonly Func<IPlayer> _createDummyPlayer;

        private IPlayer _dummyPlayer;
        private SessionState _state;
        private bool _isDestroyed;

        private abstract class SessionState
        {
            public virtual void Enter() { }
            public virtual void RequestGame(string playerGuid) { }
            public virtual void PickStone(string gameGuid, string playerGuid, int stoneType) { }
            public virtual void StartGame(string gameGuid, string playerGuid) { }
            public virtual void PutStone(string gameGuid, string playerGuid, int column, int row) { }
            public virtual void Exit() { }
        }

        private class WaitingState : SessionState
        {
            private readonly MockGameSession _session;

            public WaitingState(MockGameSession session)
            {
                _session = session;
            }

            public override void RequestGame(string playerGuid)
            {
                _session._dummyPlayer = _session._createDummyPlayer();
                _session._dummyPlayer.SetPlayerGuid(Guid.NewGuid().ToString());

                var gameGuid = Guid.NewGuid().ToString();
                _session._checkerboard.SetGameGuid(gameGuid);
                _session._checkerboard.Setup();

                _session.GameCreatedEvent?.Invoke(_session, new GameCreatedEventArgs { GameGuid = gameGuid });
            }

            public override void PickStone(string gameGuid, string playerGuid, int stoneType)
            {
                var dummyPlayer = _session._dummyPlayer;

                _session._checkerboard.RegisterStoneOwner(playerGuid, stoneType);
                var dummyPlayerStone = stoneType == StoneType.Black ? StoneType.White : StoneType.Black;
                _session._checkerboard.RegisterStoneOwner(dummyPlayer.GetPlayerGuid(), dummyPlayerStone);

                var stoneOwners = new List<StoneOwner>
                {
                    new StoneOwner { PlayerGuid = playerGuid, StoneType = stoneType },
                    new StoneOwner { PlayerGuid = dummyPlayer.GetPlayerGuid(), StoneType = dummyPlayerStone }
                };
                _session.GamePreparedEvent?.Invoke(this, new GamePreparedEventArgs { StoneOwners = stoneOwners });
            }

            public override void StartGame(string gameGuid, string playerGuid)
            {
                var newState = new InGameState(_session);
                _session.ChangeState(newState);
                newState.StartGame(gameGuid, playerGuid);
            }
        }

        private class InGameState : SessionState
        {
            private readonly MockGameSession _session;

            public InGameState(MockGameSession session)
            {
                _session = session;
            }

            public override void Enter()
            {
                _session.PlayerTurnStartedEvent += OnPlayerTurnStartedEvent;
                _session.PlayerTurnFinishedEvent += OnPlayerTurnFinishedEvent;
            }

            public override void StartGame(string gameGuid, string playerGuid)
            {
                var firstTurnPlayerGuid = _session._checkerboard.GetPlayerGuid(StoneType.Black);
                StartTurn(firstTurnPlayerGuid);
            }

            private void StartTurn(string playerGuid)
            {
                _session.PlayerTurnStartedEvent?.Invoke(this, new PlayerTurnStartedEventArgs
                {
                    PlayerGuid = playerGuid,
                    TimeLimit = _session._timeLimit
                });

                _session._timer.Start(0, _session._timeLimit, () =>
                {
                    TriggerPlayerTurnFinishedEvent(playerGuid, -1, -1, StoneType.Null);

                    StartOpponentPlayerTurn(playerGuid);
                });
            }

            private void StartOpponentPlayerTurn(string playerGuid)
            {
                var opponentPlayerGuid = _session._checkerboard.GetOpponentPlayerGuid(playerGuid);
                StartTurn(opponentPlayerGuid);
            }

            public override void PutStone(string gameGuid, string playerGuid, int column, int row)
            {
                var stoneType = _session._checkerboard.GetStone(playerGuid);

                _session._checkerboard.PutStone(column, row, stoneType);

                _session._timer.Stop(0);

                TriggerPlayerTurnFinishedEvent(playerGuid, column, row, stoneType);

                var winnerStone = _session._checkerboard.CheckWinner();
                if (winnerStone == StoneType.Null)
                {
                    StartOpponentPlayerTurn(playerGuid);
                }
                else
                {
                    _session.ChangeState(new WaitingState(_session));
                    FinishGame(winnerStone);
                }
            }

            private void TriggerPlayerTurnFinishedEvent(string playerGuid, int column, int row, int stoneType)
            {
                _session.PlayerTurnFinishedEvent?.Invoke(this, new PlayerTurnFinishedEventArgs
                {
                    PlayerGuid = playerGuid,
                    StoneType = stoneType,
                    Column = column,
                    Row = row
                });
            }

            private void FinishGame(int winnerStone)
            {
                var winnerGuid = _session._checkerboard.GetPlayerGuid(winnerStone);
                _session._checkerboard.Clear();

                _session.GameFinishedEvent?.Invoke(this, new GameFinishedEventArgs
                {
                    WinnerGuid = winnerGuid,
                    WinnerStone = winnerStone
                });
            }

            private void OnPlayerTurnStartedEvent(object sender, PlayerTurnStartedEventArgs args)
            {
                if (args.PlayerGuid != _session._dummyPlayer.GetPlayerGuid())
                    return;

                _session._dummyPlayer.StartTurn();
            }

            private void OnPlayerTurnFinishedEvent(object sender, PlayerTurnFinishedEventArgs args)
            {
                if (args.PlayerGuid != _session._dummyPlayer.GetPlayerGuid())
                    return;

                _session._dummyPlayer.HaltTurn();
            }

            public override void Exit()
            {
                _session.PlayerTurnStartedEvent -= OnPlayerTurnStartedEvent;
                _session.PlayerTurnFinishedEvent -= OnPlayerTurnFinishedEvent;
                _session._timer.Stop(0);
                _session._dummyPlayer?.Destroy();
                _session._dummyPlayer = null;
            }
        }

        public MockGameSession(Timer timer, Entity.Checkerboard checkerboard, Func<IPlayer> createDummyPlayer)
        {
            _timeLimit = 60;
            _timer = timer;
            _checkerboard = checkerboard;
            _createDummyPlayer = createDummyPlayer;
            _isDestroyed = false;
        }

        private void ChangeState(SessionState newState)
        {
            _state?.Exit();
            _state = newState;
            _state.Enter();
        }

        public void RequestConnect()
        {
            ChangeState(new WaitingState(this));

            var playerGuid = Guid.NewGuid().ToString();
            ConnectedEvent?.Invoke(this, new ConnectedEventArgs { PlayerGuid = playerGuid });
        }

        public void RequestGame(string playerGuid)
        {
            _state?.RequestGame(playerGuid);
        }

        public void PickStone(string gameGuid, string playerGuid, int stoneType)
        {
            _state?.PickStone(gameGuid, playerGuid, stoneType);
        }

        public void StartGame(string gameGuid, string playerGuid)
        {
            _state?.StartGame(gameGuid, playerGuid);
        }

        public void PutStone(string gameGuid, string playerGuid, int column, int row)
        {
            _state?.PutStone(gameGuid, playerGuid, column, row);
        }

        public void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _state?.Exit();
            _state = null;

            _timer.Destroy();
        }
    }
}
