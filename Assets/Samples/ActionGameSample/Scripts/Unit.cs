using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class Unit : Actor
    {
        private readonly IUnitAvatar avatar;
        private readonly AbilitySlot abilitySlot = new();

        public IUnitAvatar Avatar => avatar;
        public AbilitySlot AbilitySlot => abilitySlot;

        public Unit(IUnitAvatar avatar, AbilitySystem abilitySystem) : base(abilitySystem)
        {
            this.avatar = avatar;
        }

        public bool IsControllable()
        {
            return GetStat(StatId.CONTROLLABLE).CurrentValue == 1;
        }

        public void Move(float x, float y)
        {
            float speedDelta = GetStat(StatId.SPEED).CurrentValue * 0.01f * Time.deltaTime;
            Avatar.Move(x * speedDelta, y * speedDelta);
        }

        public void Tick()
        {
            abilitySlot.Tick();
        }
    }
}
