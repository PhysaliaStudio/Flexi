using UnityEngine;

namespace Physalia.AbilitySystem.Tests
{
    public class LogCharacterNameNode : ProcessNode
    {
        public Inport<Character> character;

        protected override AbilityState DoLogic()
        {
            Debug.Log($"My name is {character.GetValue()?.name}");
            return AbilityState.RUNNING;
        }
    }
}
