namespace Liz.Core.Progress;

/// <summary>
///     Component which handles progress updates from the liz extract-licenses process
/// </summary>
public interface IProgressHandler
{
    /// <summary>
    ///     Initializes the overall main-process
    /// </summary>
    /// <param name="totalSteps">The total steps this process will have</param>
    /// <param name="initialMessage">The initial message of the process</param>
    void InitializeMainProcess(int totalSteps, string initialMessage);
    
    /// <summary>
    ///     Advances the main-process by one step
    /// </summary>
    /// <param name="message">A message for the current sub-process</param>
    void TickMainProcess(string message);
    
    /// <summary>
    ///     Indicates that the main-process has finished
    /// </summary>
    void FinishMainProcess();

    /// <summary>
    ///     Initializes a new sub-process
    /// </summary>
    /// <param name="totalSteps">The total steps this process will have</param>
    void InitializeNewSubProcess(int totalSteps);
    
    /// <summary>
    ///     Advances the current sub-process by one step
    /// </summary>
    /// <param name="message">A message for the current sub-sub-process</param>
    void TickCurrentSubProcess(string message);
}
