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
