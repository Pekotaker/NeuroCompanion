using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace NeuroCompanion.Configs
{
    public class NeuroCompanionConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue("ws://localhost:8000")]
        public string RandyWebSocketUrl { get; set; }

        [DefaultValue(false)]
        public bool AutoConnectOnWorldLoad { get; set; }

        [DefaultValue(5)]
        public int PeriodicContextIntervalSeconds { get; set; }

        [DefaultValue(true)]
        public bool EnableEventContextMessages { get; set; }

        [DefaultValue(35)]
        public int LowHealthPercent { get; set; }

        [DefaultValue(2)]
        public int RecallCooldownSeconds { get; set; }

        [DefaultValue(1)]
        public int FollowCooldownSeconds { get; set; }

        [DefaultValue(3)]
        public int AttackOnceCooldownSeconds { get; set; }

        [DefaultValue(5)]
        public int AutoAttackCooldownSeconds { get; set; }

        [DefaultValue(60)]
        public int BuffPlayerCooldownSeconds { get; set; }

        [DefaultValue(60)]
        public int DebuffPlayerCooldownSeconds { get; set; }

        [DefaultValue(30)]
        public int DebuffEnemyCooldownSeconds { get; set; }
    }
}