namespace LikeLion.LH1.Client.Core
{
    public interface ILoop
    {
        void Add(IUpdatable updatable);
        void Remove(IUpdatable updatable);
    }
}
