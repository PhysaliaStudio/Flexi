namespace Physalia.Flexi
{
    internal class AbilityFactory : ObjectInstanceFactory<Ability>
    {
        private readonly AbilitySystem abilitySystem;
        private readonly AbilityDataSource abilityDataSource;

        public override string Name => abilityDataSource.ToString();

        public AbilityFactory(AbilitySystem abilitySystem, AbilityDataSource abilityDataSource)
        {
            this.abilitySystem = abilitySystem;
            this.abilityDataSource = abilityDataSource;
        }

        public override Ability Create()
        {
            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            return ability;
        }

        public override void Reset(Ability ability)
        {
            ability.Reset();
        }
    }
}
