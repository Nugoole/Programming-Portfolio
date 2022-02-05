namespace ALT.CVL.Log.Interface
{
    public interface ILog
    {
        string LogDate { get; }
        string LogTime { get; }
        string LogLevel { get; }
        string LogMessage { get; }
    }
}
