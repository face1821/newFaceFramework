using System;
using UnityEngine;

/// <summary>
/// 控制台单条日志数据。
/// </summary>
public class ConsoleLogEntry
{
    public LogType Type;
    public string Message;
    public string StackTrace;
    public DateTime Time;

    public ConsoleLogEntry(LogType type, string message, string stackTrace = null)
    {
        Type = type;
        Message = message ?? string.Empty;
        StackTrace = stackTrace ?? string.Empty;
        Time = DateTime.Now;
    }
}
