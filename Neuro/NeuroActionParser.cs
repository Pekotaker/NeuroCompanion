using System.Text.Json.Nodes;

namespace NeuroCompanion.Neuro
{
    public static class NeuroActionParser
    {
        private const int DefaultAutoAttackDurationSeconds = 10;

        public static bool TryCreateCommandFromAction(
            JsonNode data,
            out NeuroCommand command,
            out string error
        )
        {
            command = null;
            error = null;

            string actionName = data?["name"]?.GetValue<string>();

            switch (actionName)
            {
                case NeuroActionNames.RecallCompanion:
                    command = new NeuroCommand(NeuroCommandType.Recall);
                    return true;

                case NeuroActionNames.Follow:
                    command = new NeuroCommand(NeuroCommandType.Follow);
                    return true;

                case NeuroActionNames.AttackOnce:
                    command = new NeuroCommand(NeuroCommandType.AttackOnce);
                    return true;

                case NeuroActionNames.AutoAttack:
                    command = new NeuroCommand(
                        NeuroCommandType.StartTimedAttack,
                        GetDurationSecondsFromActionData(data)
                    );
                    return true;

                case NeuroActionNames.BuffPlayer:
                    command = new NeuroCommand(NeuroCommandType.BuffPlayer);
                    return true;

                case NeuroActionNames.DebuffPlayer:
                    command = new NeuroCommand(NeuroCommandType.DebuffPlayer);
                    return true;

                case NeuroActionNames.DebuffEnemy:
                    command = new NeuroCommand(NeuroCommandType.DebuffNearestEnemy);
                    return true;

                default:
                    error = $"Unknown Neuro action: {actionName}";
                    return false;
            }
        }

        private static int GetDurationSecondsFromActionData(JsonNode data)
        {
            string actionDataJson = data?["data"]?.GetValue<string>();

            if (string.IsNullOrWhiteSpace(actionDataJson))
            {
                return DefaultAutoAttackDurationSeconds;
            }

            try
            {
                JsonNode actionData = JsonNode.Parse(actionDataJson);

                int? durationSeconds =
                    actionData?["duration_seconds"]?.GetValue<int>();

                return durationSeconds ?? DefaultAutoAttackDurationSeconds;
            }
            catch
            {
                return DefaultAutoAttackDurationSeconds;
            }
        }
    }
}