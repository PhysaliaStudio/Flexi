using UnityEngine;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Common)]
    public class LogNode : ProcessNode
    {
        public Inport<string> text;

        protected override AbilityState OnExecute()
        {
            Debug.Log(text.GetValue());
            return AbilityState.RUNNING;
        }
    }
}
