using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace NeuroCompanion.Configs
{
    public class NeuroCompanionConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Connection")]
        [DefaultValue("ws://localhost:8000")]
        public string RandyWebSocketUrl { get; set; }

        [DefaultValue(false)]
        public bool AutoConnectOnWorldLoad { get; set; }

        [Header("Context")]
        [Range(1, 60)]
        [Slider]
        [DefaultValue(5)]
        public int PeriodicContextIntervalSeconds { get; set; }

        [DefaultValue(true)]
        public bool EnableEventContextMessages { get; set; }

        [Range(1, 100)]
        [Slider]
        [DefaultValue(35)]
        public int LowHealthPercent { get; set; }

        [Header("ActionCooldowns")]
        [Range(0, 30)]
        [Slider]
        [DefaultValue(2)]
        public int RecallCooldownSeconds { get; set; }

        [Range(0, 30)]
        [Slider]
        [DefaultValue(1)]
        public int FollowCooldownSeconds { get; set; }

        [Range(0, 30)]
        [Slider]
        [DefaultValue(3)]
        public int AttackOnceCooldownSeconds { get; set; }

        [Range(0, 60)]
        [Slider]
        [DefaultValue(5)]
        public int AutoAttackCooldownSeconds { get; set; }

        [Range(0, 300)]
        [Slider]
        [DefaultValue(60)]
        public int BuffPlayerCooldownSeconds { get; set; }

        [Range(0, 300)]
        [Slider]
        [DefaultValue(60)]
        public int DebuffPlayerCooldownSeconds { get; set; }

        [Range(0, 300)]
        [Slider]
        [DefaultValue(30)]
        public int DebuffEnemyCooldownSeconds { get; set; }
    }
}