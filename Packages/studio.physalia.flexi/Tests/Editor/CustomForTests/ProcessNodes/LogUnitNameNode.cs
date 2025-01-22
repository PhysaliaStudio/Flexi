using UnityEngine;

namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class LogCharacterNameNode : DefaultProcessNode
    {
        public Inport<CustomUnit> character;

        protected override FlowState OnExecute()
        {
            Debug.Log($"My name is {character.GetValue()?.Name}");
            return FlowState.Success;
        }
    }
}
