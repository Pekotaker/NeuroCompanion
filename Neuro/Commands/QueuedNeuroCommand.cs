namespace NeuroCompanion.Neuro
{
    public class QueuedNeuroCommand
    {
        public string ActionId { get; }
        public NeuroCommand Command { get; }

        public QueuedNeuroCommand(string actionId, NeuroCommand command)
        {
            ActionId = actionId;
            Command = command;
        }
    }
}