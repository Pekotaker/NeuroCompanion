using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro.Effects
{
    public static class NeuroBuffLookup
    {
        private const int NoSpecificBuff = -1;

        public static bool TryFindAllowedBuff(
            string input,
            int[] allowedBuffs,
            string listName,
            out int buffId,
            out string error
        )
        {
            buffId = NoSpecificBuff;
            error = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = $"No {listName} was provided.";
                return false;
            }

            input = input.Trim();

            if (int.TryParse(input, out int parsedId))
            {
                if (IsAllowed(parsedId, allowedBuffs))
                {
                    buffId = parsedId;
                    return true;
                }

                error =
                    $"{parsedId} is not an allowed Neuro {listName}. Allowed: {FormatAllowedBuffList(allowedBuffs)}";
                return false;
            }

            string normalizedInput = NormalizeBuffName(input);

            foreach (int allowedBuff in allowedBuffs)
            {
                string normalizedBuffName =
                    NormalizeBuffName(GetBuffNameSafe(allowedBuff));

                if (normalizedInput == normalizedBuffName)
                {
                    buffId = allowedBuff;
                    return true;
                }
            }

            error =
                $"Unknown Neuro {listName}: {input}. Allowed: {FormatAllowedBuffList(allowedBuffs)}";
            return false;
        }

        public static bool IsAllowed(int buffId, int[] allowedBuffs)
        {
            foreach (int allowedBuff in allowedBuffs)
            {
                if (buffId == allowedBuff)
                {
                    return true;
                }
            }

            return false;
        }

        public static string FormatBuffList(List<int> buffs)
        {
            if (buffs.Count == 0)
            {
                return "none";
            }

            StringBuilder builder = new();

            for (int i = 0; i < buffs.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(GetBuffNameSafe(buffs[i]));
            }

            return builder.ToString();
        }

        public static string FormatAllowedBuffList(int[] buffs)
        {
            StringBuilder builder = new();

            for (int i = 0; i < buffs.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                int buff = buffs[i];

                builder.Append(GetBuffNameSafe(buff));
                builder.Append(" (");
                builder.Append(buff);
                builder.Append(")");
            }

            return builder.ToString();
        }

        public static string GetBuffNameSafe(int buffId)
        {
            if (buffId < 0 || buffId >= BuffLoader.BuffCount)
            {
                return $"Unknown buff {buffId}";
            }

            return Lang.GetBuffName(buffId);
        }

        private static string NormalizeBuffName(string text)
        {
            StringBuilder builder = new();

            foreach (char character in text)
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(char.ToLowerInvariant(character));
                }
            }

            return builder.ToString();
        }
    }
}