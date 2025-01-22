namespace Physalia.Flexi
{
    internal class AbilityFactory : ObjectInstanceFactory<Ability>
    {
        private readonly AbilitySystem abilitySystem;
        private readonly AbilityHandle abilityHandle;

        public override string Name => abilityHandle.ToString();

        public AbilityFactory(AbilitySystem abilitySystem, AbilityHandle abilityHandle)
        {
            this.abilitySystem = abilitySystem;
            this.abilityHandle = abilityHandle;
        }

        public override Ability Create()
        {
            var ability = new Ability(abilitySystem, abilityHandle);
            ability.Initialize();
            return ability;
        }

        public override void Reset(Ability ability)
        {
            ability.Reset();
        }
    }
}
