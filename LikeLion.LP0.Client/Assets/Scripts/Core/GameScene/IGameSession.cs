using System;

namespace LikeLion.LH1.Client.Core.GameScene
{
    public interface IGameSession
    {
        event EventHandler<ConnectedEventArgs> ConnectedEvent;
        event EventHandler<GameCreatedEventArgs> GameCreatedEvent;
        event EventHandler<GamePreparedEventArgs> GamePreparedEvent;
        event EventHandler<PlayerTurnStartedEventArgs> PlayerTurnStartedEvent;
        event EventHandler<PlayerTurnFinishedEventArgs> PlayerTurnFinishedEvent;
        event EventHandler<GameFinishedEventArgs> GameFinishedEvent;

        void RequestConnect();
        void RequestGame(string playerGuid);
        void PickStone(string gameGuid, string playerGuid, int stoneType);
        void PutStone(string gameGuid, string playerGuid, int column, int row);
        void StartGame(string gameGuid, string playerGuid);
        void Destroy();
    }
}
