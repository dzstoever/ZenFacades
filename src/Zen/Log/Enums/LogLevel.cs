namespace Zen.Log
{
    /// <summary>
    /// Fatal   , logs -> FATAL 
    /// Error   , logs -> FATAL ERROR
    /// Warn    , logs -> FATAL ERROR WARN
    /// Info    , logs -> FATAL ERROR WARN INFO 
    /// Debug   , logs -> FATAL ERROR WARN INFO DEBUG
    /// </summary>
    /// <remarks>
    /// min= Debug := Off 
    /// max= Fatal := All
    /// </remarks>
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
        All,
        Off,
    }
}