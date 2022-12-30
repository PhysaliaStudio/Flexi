using UnityEngine;

namespace Physalia.Flexi
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
