using System;

namespace Physalia.AbilityFramework
{
    [Serializable]
    public class BlackboardVariable
    {
        public string key = "";
        public int value;

        public BlackboardVariable Clone()
        {
            return new BlackboardVariable
            {
                key = key,
                value = value,
            };
        }
    }
}
