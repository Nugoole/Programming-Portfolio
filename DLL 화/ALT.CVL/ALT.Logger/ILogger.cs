using ALT.CVL.Log.Enum;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace ALT.CVL.Log.Interface
{
    /// <summary>
    /// Log를 남기는 객체에 대한 인터페이스 입니다.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 현재 기록된 Log들 입니다.
        /// </summary>
        IReadOnlyCollection<ILog> Logs { get; }


        bool IsReversed { get; set; }

        /// <summary>
        /// 현재 볼 수 있는 최대 Log 개수 입니다.
        /// => 파일로 저장된 Log는 계속 늘어나지만 현재 List에 들고 있는 Log 개수는 Capcity가 나타냅니다.
        /// </summary>
        int Capacity { get; set; }

        /// <summary>
        /// Logger가 기록하는 Log파일의 위치
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// 새로운 Log를 씁니다.
        /// </summary>
        /// <param name="logLevel">
        /// Log의 LogLevel입니다.
        /// </param>
        /// <param name="logMessage">
        /// Log에 저장할 메시지 입니다.
        /// </param>
        void Write(LogLevel logLevel, string logMessage);

        /// <summary>
        /// (비동기)새로운 Log를 씁니다.
        /// </summary>
        /// <param name="logLevel">
        /// Log의 LogLevel입니다.
        /// </param>
        /// <param name="logMessage">
        /// Log에 저장할 메시지 입니다.
        /// </param>
        /// <returns></returns>
        Task<bool> WriteAsync(LogLevel logLevel, string logMessage);

        /// <summary>
        /// 현재 로그를 비웁니다.
        /// </summary>
        void ClearCurrentLogs();
    }
}
