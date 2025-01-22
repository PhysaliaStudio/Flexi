namespace Physalia.Flexi
{
    internal class AbilityFactory : ObjectInstanceFactory<Ability>
    {
        private readonly FlexiCore flexiCore;
        private readonly AbilityHandle abilityHandle;

        public override string Name => abilityHandle.ToString();

        public AbilityFactory(FlexiCore flexiCore, AbilityHandle abilityHandle)
        {
            this.flexiCore = flexiCore;
            this.abilityHandle = abilityHandle;
        }

        public override Ability Create()
        {
            var ability = new Ability(flexiCore, abilityHandle);
            ability.Initialize();
            return ability;
        }

        public override void Reset(Ability ability)
        {
            ability.Reset();
        }
    }
}
