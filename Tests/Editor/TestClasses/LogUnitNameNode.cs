using UnityEngine;

namespace Physalia.AbilityFramework.Tests
{
    [NodeCategory("Built-in/[Test Custom]")]
    public class LogCharacterNameNode : ProcessNode
    {
        public Inport<CustomUnit> character;

        protected override AbilityState DoLogic()
        {
            Debug.Log($"My name is {character.GetValue()?.Name}");
            return AbilityState.RUNNING;
        }
    }
}
