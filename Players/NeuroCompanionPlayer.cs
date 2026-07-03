using Terraria;
using Terraria.ModLoader.IO;
using NeuroCompanion.Buffs;
using NeuroCompanion.Neuro;
using Terraria.ModLoader;

namespace NeuroCompanion.Players
{
    public class NeuroCompanionPlayer : ModPlayer
    {
        private bool recallRequested;
        private bool singleAttackRequested;
        private bool attackPlayerRequested;
        private int evilVisualTicksRemaining;

        public const int DefaultNeuroStaffShootCooldownTicks = 50;
        public const int EvilVisualDurationTicks = 180;

        public Item NeuroWeapon { get; private set; }
        public int NeuroStaffPrefix { get; set; }

        public int NeuroStaffVisualTier { get; set; }

        public bool NeuroStaffCanDetectThroughBlocks { get; set; }

        public int NeuroStaffShootCooldownTicks { get; set; }

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

            attackPlayerRequested = false;
            evilVisualTicksRemaining = 0;

            NeuroStaffPrefix = 0;
            NeuroStaffVisualTier = 1;
            NeuroStaffShootCooldownTicks = DefaultNeuroStaffShootCooldownTicks;
            NeuroStaffCanDetectThroughBlocks = false;

            NeuroWeapon = new Item();
            NeuroWeapon.TurnToAir();
        }

        public bool HasNeuroWeapon()
        {
            return NeuroWeapon != null && !NeuroWeapon.IsAir;
        }

        public void SetNeuroWeapon(Item item)
        {
            NeuroWeapon = item.Clone();
        }

        public void ClearNeuroWeapon()
        {
            NeuroWeapon = new Item();
            NeuroWeapon.TurnToAir();
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

        public void RequestAttackPlayer()
        {
            attackPlayerRequested = true;
        }

        public bool ConsumeAttackPlayerRequest()
        {
            if (!attackPlayerRequested)
            {
                return false;
            }

            attackPlayerRequested = false;
            return true;
        }

        public void TriggerEvilVisual()
        {
            evilVisualTicksRemaining = EvilVisualDurationTicks;
        }

        public bool ShouldUseEvilSprite()
        {
            return evilVisualTicksRemaining > 0;
        }

        public override void PostUpdate()
        {
            if (evilVisualTicksRemaining > 0)
            {
                evilVisualTicksRemaining--;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (HasNeuroWeapon())
            {
                tag["NeuroWeapon"] = ItemIO.Save(NeuroWeapon);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("NeuroWeapon"))
            {
                NeuroWeapon = ItemIO.Load(tag.GetCompound("NeuroWeapon"));
            }
            else
            {
                ClearNeuroWeapon();
            }
        }
    }
}