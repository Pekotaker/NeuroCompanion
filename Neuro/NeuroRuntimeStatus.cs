using System;

namespace NeuroCompanion.Neuro
{
    public static class NeuroRuntimeStatus
    {
        private static readonly object LockObject = new();

        private static string lastReceivedAction = "None.";
        private static string lastQueuedCommand = "None.";
        private static string lastExecutedCommand = "None.";
        private static string lastActionResult = "None.";
        private static string lastContextSent = "None.";

        public static void RecordReceivedAction(string actionId, string actionName)
        {
            lock (LockObject)
            {
                lastReceivedAction =
                    $"{Timestamp()} Randy action received: id={actionId}, name={actionName}";
            }
        }

        public static void RecordQueuedCommand(string actionId, NeuroCommand command)
        {
            lock (LockObject)
            {
                lastQueuedCommand =
                    $"{Timestamp()} Queued command: id={actionId}, type={command.Type}, duration={command.DurationSeconds}s";
            }
        }

        public static void RecordExecutedCommand(
            string actionId,
            NeuroCommand command,
            NeuroActionResult result
        )
        {
            lock (LockObject)
            {
                lastExecutedCommand =
                    $"{Timestamp()} Executed command: id={actionId}, type={command.Type}";

                lastActionResult =
                    $"{Timestamp()} Result: success={result.Success}, message={result.Message}";
            }
        }

        public static void RecordContextSent()
        {
            lock (LockObject)
            {
                lastContextSent = $"{Timestamp()} Terraria context sent.";
            }
        }

        public static string BuildDebugText()
        {
            lock (LockObject)
            {
                return
                    "Neuro runtime debug:\n" +
                    $"- Last received action: {lastReceivedAction}\n" +
                    $"- Last queued command: {lastQueuedCommand}\n" +
                    $"- Last executed command: {lastExecutedCommand}\n" +
                    $"- Last action result: {lastActionResult}\n" +
                    $"- Last context sent: {lastContextSent}";
            }
        }

        private static string Timestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}