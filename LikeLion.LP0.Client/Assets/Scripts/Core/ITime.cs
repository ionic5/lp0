namespace LikeLion.LH1.Client.Core
{
    public interface ITime
    {
        long GetCurrentTimeMilliseconds();
        long GetCurrentTime();
        float GetDeltaTime();
        long GetToday();
    }
}
