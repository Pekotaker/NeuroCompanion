using System.Collections.Concurrent;

namespace NeuroCompanion.Neuro
{
    public static class NeuroCommandQueue
    {
        private static readonly ConcurrentQueue<QueuedNeuroCommand> Commands = new();

        public static void Enqueue(QueuedNeuroCommand command)
        {
            Commands.Enqueue(command);
        }

        public static bool TryDequeue(out QueuedNeuroCommand command)
        {
            return Commands.TryDequeue(out command);
        }
    }
}