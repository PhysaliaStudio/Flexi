using System;

namespace Physalia.Flexi
{
    public abstract class AbilityContainer
    {
        private AbilityHandle _handle;

        internal AbilityHandle Handle => _handle;

        public AbilityContainer(AbilityData abilityData, int groupIndex)
        {
            SetAbilityDataGroup(abilityData, groupIndex);
        }

        public void SetAbilityDataGroup(AbilityData abilityData, int groupIndex)
        {
            if (abilityData == null)
            {
                throw new ArgumentNullException("abilityData is null");
            }

            if (groupIndex < 0 || groupIndex >= abilityData.graphGroups.Count)
            {
                throw new IndexOutOfRangeException();
            }

            _handle = new AbilityHandle(abilityData, groupIndex);
        }
    }
}
