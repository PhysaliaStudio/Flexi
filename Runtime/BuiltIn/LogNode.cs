using UnityEngine;

namespace Physalia.AbilitySystem
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
