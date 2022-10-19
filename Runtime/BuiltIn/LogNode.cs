using UnityEngine;

namespace Physalia.AbilityFramework
{
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
