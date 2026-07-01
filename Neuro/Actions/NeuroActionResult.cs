namespace NeuroCompanion.Neuro
{
    public class NeuroActionResult
    {
        public bool Success { get; }
        public string Message { get; }

        private NeuroActionResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static NeuroActionResult Ok(string message)
        {
            return new NeuroActionResult(true, message);
        }

        public static NeuroActionResult Fail(string message)
        {
            return new NeuroActionResult(false, message);
        }
    }
}