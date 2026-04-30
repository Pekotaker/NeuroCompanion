using NeuroCompanion.Neuro;
using Terraria.ModLoader;

namespace NeuroCompanion.Players
{
    public class NeuroCompanionPlayer : ModPlayer
    {
        private bool recallRequested;

        public NeuroCompanionMode CompanionMode { get; private set; }

        public override void Initialize()
        {
            CompanionMode = NeuroCompanionMode.AttackNearest;
            recallRequested = false;
        }

        public void SetCompanionMode(NeuroCompanionMode mode)
        {
            CompanionMode = mode;
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
    }
}