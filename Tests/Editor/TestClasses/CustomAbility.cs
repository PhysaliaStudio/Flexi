using UnityEngine;

namespace Physalia.AbilitySystem.Tests
{
    public static class CustomAbility
    {
        public static string HELLO_WORLD
        {
            get
            {
                return ReadAbilityFile("HelloWorld");
            }
        }

        public static string NORAML_ATTACK
        {
            get
            {
                return ReadAbilityFile("NormalAttack");
            }
        }

        public static string ATTACK_DECREASE
        {
            get
            {
                return ReadAbilityFile("AttackDecrease");
            }
        }

        private static string ReadAbilityFile(string fileName)
        {
            TextAsset asset = Resources.Load<TextAsset>(fileName);
            if (asset == null)
            {
                return "";
            }

            return asset.text;
        }
    }
}
