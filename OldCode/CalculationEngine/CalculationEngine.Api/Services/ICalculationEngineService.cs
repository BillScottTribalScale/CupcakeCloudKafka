namespace CalculationEngine.Api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICalculationEngineService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageContent"></param>
        /// <returns></returns>
        bool StartProcess (string messageContent);
    }
}
