using Terraria;

namespace NeuroCompanion.Neuro
{
    public static class NeuroNpcTargetFinder
    {
        public static NPC FindNearestEnemy(
            Player player,
            float searchRange
        )
        {
            if (player == null || !player.active)
            {
                return null;
            }

            NPC bestTarget = null;
            float bestDistance = searchRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!IsValidEnemy(npc))
                {
                    continue;
                }

                float distance = player.Distance(npc.Center);

                if (distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestTarget = npc;
            }

            return bestTarget;
        }

        public static bool IsValidEnemy(NPC npc)
        {
            return npc != null &&
                   npc.active &&
                   npc.CanBeChasedBy();
        }
    }
}