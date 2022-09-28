using UnityEngine;

namespace Physalia.AbilitySystem.Tests
{
    public class LogCharacterNameNode : ProcessNode
    {
        public Inport<Character> character;

        protected override void DoLogic()
        {
            Debug.Log($"My name is {character.GetValue()?.name}");
        }
    }
}
