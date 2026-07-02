using System.Collections.Concurrent;

namespace NeuroCompanion.Neuro.Runtime
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