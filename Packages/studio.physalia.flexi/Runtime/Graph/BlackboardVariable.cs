using System;

namespace Physalia.Flexi
{
    [Serializable]
    public class BlackboardVariable
    {
        public string key = "";
        public int value;

        internal BlackboardVariable Clone()
        {
            return new BlackboardVariable
            {
                key = key,
                value = value,
            };
        }
    }
}
