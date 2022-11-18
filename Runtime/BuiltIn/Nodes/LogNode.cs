using UnityEngine;

namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Common")]
    public class LogNode : ProcessNode
    {
        public Inport<string> text;

        protected override AbilityState DoLogic()
        {
            Debug.Log(text.GetValue());
            return AbilityState.RUNNING;
        }
    }
}
