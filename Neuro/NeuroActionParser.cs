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

                case NeuroActionNames.AttackPlayer:
                    command = new NeuroCommand(NeuroCommandType.AttackPlayer);
                    return true;

                case NeuroActionNames.AutoAttack:
                    command = new NeuroCommand(
                        NeuroCommandType.StartTimedAttack,
                        GetDurationSecondsFromActionData(data)
                    );
                    return true;

                case NeuroActionNames.BuffPlayer:
                    return TryCreateBuffPlayerCommand(
                        data,
                        out command,
                        out error
                    );

                case NeuroActionNames.DebuffPlayer:
                    return TryCreateDebuffCommand(
                        data,
                        NeuroCommandType.DebuffPlayer,
                        out command,
                        out error
                    );

                case NeuroActionNames.DebuffEnemy:
                    return TryCreateDebuffCommand(
                        data,
                        NeuroCommandType.DebuffNearestEnemy,
                        out command,
                        out error
                    );

                case NeuroActionNames.WeaponStatus:
                    command = new NeuroCommand(NeuroCommandType.WeaponStatus);
                    return true;

                case NeuroActionNames.EquipWeaponFromInventory:
                    command = new NeuroCommand(NeuroCommandType.EquipWeaponFromInventory);
                    return true;

                case NeuroActionNames.ReturnWeaponToPlayer:
                    command = new NeuroCommand(NeuroCommandType.ReturnWeaponToPlayer);
                    return true;

                default:
                    error = $"Unknown Neuro action: {actionName}";
                    return false;
            }
        }

        private static bool TryCreateBuffPlayerCommand(
            JsonNode data,
            out NeuroCommand command,
            out string error
        )
        {
            command = null;
            error = null;

            string buffInput = GetOptionalActionString(data, "buff");

            if (string.IsNullOrWhiteSpace(buffInput))
            {
                command = new NeuroCommand(NeuroCommandType.BuffPlayer);
                return true;
            }

            if (!NeuroPotionEffects.TryFindPositiveBuff(
                    buffInput,
                    out int buffId,
                    out error
                ))
            {
                return false;
            }

            command = new NeuroCommand(
                NeuroCommandType.BuffPlayer,
                effectBuffId: buffId
            );

            return true;
        }

        private static bool TryCreateDebuffCommand(
            JsonNode data,
            NeuroCommandType commandType,
            out NeuroCommand command,
            out string error
        )
        {
            command = null;
            error = null;

            string debuffInput = GetOptionalActionString(data, "debuff");

            if (string.IsNullOrWhiteSpace(debuffInput))
            {
                command = new NeuroCommand(commandType);
                return true;
            }

            if (!NeuroPotionEffects.TryFindDebuff(
                    debuffInput,
                    out int debuffId,
                    out error
                ))
            {
                return false;
            }

            command = new NeuroCommand(
                commandType,
                effectBuffId: debuffId
            );

            return true;
        }

        private static int GetDurationSecondsFromActionData(JsonNode data)
        {
            JsonNode actionData = ParseActionData(data);

            if (actionData == null)
            {
                return DefaultAutoAttackDurationSeconds;
            }

            int? durationSeconds =
                actionData["duration_seconds"]?.GetValue<int>();

            return durationSeconds ?? DefaultAutoAttackDurationSeconds;
        }

        private static string GetOptionalActionString(
            JsonNode data,
            string propertyName
        )
        {
            JsonNode actionData = ParseActionData(data);

            if (actionData == null)
            {
                return null;
            }

            JsonNode valueNode = actionData[propertyName];

            if (valueNode == null)
            {
                return null;
            }

            try
            {
                return valueNode.GetValue<string>();
            }
            catch
            {
                try
                {
                    return valueNode.GetValue<int>().ToString();
                }
                catch
                {
                    return null;
                }
            }
        }

        private static JsonNode ParseActionData(JsonNode data)
        {
            string actionDataJson = data?["data"]?.GetValue<string>();

            if (string.IsNullOrWhiteSpace(actionDataJson))
            {
                return null;
            }

            try
            {
                return JsonNode.Parse(actionDataJson);
            }
            catch
            {
                return null;
            }
        }
    }
}