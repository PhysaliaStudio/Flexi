using UnityEngine;

namespace Physalia.AbilitySystem
{
    public class LogNode : ProcessNode
    {
        public Inport<string> text;

        protected override void DoLogic()
        {
            Debug.Log(text.GetValue());
        }
    }
}
