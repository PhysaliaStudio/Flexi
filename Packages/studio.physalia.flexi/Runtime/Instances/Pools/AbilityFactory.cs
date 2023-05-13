namespace Physalia.Flexi
{
    internal class AbilityFactory : ObjectInstanceFactory<Ability>
    {
        private readonly AbilitySystem abilitySystem;
        private readonly AbilityData abilityData;

        public override string Name => abilityData.name;

        public AbilityFactory(AbilitySystem abilitySystem, AbilityData abilityData)
        {
            this.abilitySystem = abilitySystem;
            this.abilityData = abilityData;
        }

        public override Ability Create()
        {
            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            return ability;
        }

        public override void Reset(Ability ability)
        {
            ability.Reset();
        }
    }
}
