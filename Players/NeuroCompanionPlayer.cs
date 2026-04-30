using NeuroCompanion.Buffs;
using NeuroCompanion.Neuro;
using Terraria.ModLoader;

namespace NeuroCompanion.Players
{
    public class NeuroCompanionPlayer : ModPlayer
    {
        private bool recallRequested;
        private bool singleAttackRequested;

        public NeuroCompanionMode CompanionMode
        {
            get
            {
                if (Player.HasBuff(ModContent.BuffType<NeuroCompanionAttackBuff>()))
                {
                    return NeuroCompanionMode.TimedAttack;
                }

                return NeuroCompanionMode.Follow;
            }
        }

        public override void Initialize()
        {
            recallRequested = false;
            singleAttackRequested = false;
        }

        public void RequestRecall()
        {
            recallRequested = true;
        }

        public bool ConsumeRecallRequest()
        {
            if (!recallRequested)
            {
                return false;
            }

            recallRequested = false;
            return true;
        }

        public void RequestSingleAttack()
        {
            singleAttackRequested = true;
        }

        public bool ConsumeSingleAttackRequest()
        {
            if (!singleAttackRequested)
            {
                return false;
            }

            singleAttackRequested = false;
            return true;
        }

        public void StartTimedAttack(int durationTicks)
        {
            Player.AddBuff(ModContent.BuffType<NeuroCompanionAttackBuff>(), durationTicks);
        }

        public void StopTimedAttack()
        {
            Player.ClearBuff(ModContent.BuffType<NeuroCompanionAttackBuff>());
        }

        public int GetTimedAttackTicksRemaining()
        {
            int attackBuffType = ModContent.BuffType<NeuroCompanionAttackBuff>();

            for (int i = 0; i < Player.buffType.Length; i++)
            {
                if (Player.buffType[i] == attackBuffType)
                {
                    return Player.buffTime[i];
                }
            }

            return 0;
        }

        public int GetTimedAttackSecondsRemaining()
        {
            const int ticksPerSecond = 60;

            int ticksRemaining = GetTimedAttackTicksRemaining();

            if (ticksRemaining <= 0)
            {
                return 0;
            }

            return ticksRemaining / ticksPerSecond;
        }
    }
}