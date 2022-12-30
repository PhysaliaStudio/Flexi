using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public static class CustomAbility
    {
        public static AbilityAsset HELLO_WORLD
        {
            get
            {
                return LoadAbilityAsset("HelloWorld");
            }
        }

        public static AbilityAsset HELLO_WORLD_MACRO_CALLER
        {
            get
            {
                return LoadAbilityAsset("HelloWorld_MacroCaller");
            }
        }

        public static AbilityAsset HELLO_WORLD_MACRO_CALLER_5_TIMES
        {
            get
            {
                return LoadAbilityAsset("HelloWorld_MacroCaller5Times");
            }
        }

        public static MacroAsset HELLO_WORLD_MACRO
        {
            get
            {
                return ReadMacroAsset("HelloWorld_Macro");
            }
        }

        public static AbilityAsset HELLO_WORLD_MISSING_ELEMENTS
        {
            get
            {
                return LoadAbilityAsset("HelloWorld_MissingElements");
            }
        }

        public static AbilityAsset NORAML_ATTACK
        {
            get
            {
                return LoadAbilityAsset("NormalAttack");
            }
        }

        public static AbilityAsset NORAML_ATTACK_SELECTION
        {
            get
            {
                return LoadAbilityAsset("NormalAttackSelection");
            }
        }

        public static AbilityAsset NORMAL_ATTACK_5_TIMES
        {
            get
            {
                return LoadAbilityAsset("NormalAttack5Times");
            }
        }

        public static AbilityAsset ATTACK_DECREASE
        {
            get
            {
                return LoadAbilityAsset("AttackDecrease");
            }
        }

        public static AbilityAsset ATTACK_DOUBLE
        {
            get
            {
                return LoadAbilityAsset("AttackDouble");
            }
        }

        public static AbilityAsset ATTACK_UP_WHEN_LOW_HEALTH
        {
            get
            {
                return LoadAbilityAsset("AttackUpWhenLowHealth");
            }
        }

        public static AbilityAsset ATTACK_DOUBLE_WHEN_DAMAGED
        {
            get
            {
                return LoadAbilityAsset("AttackDoubleWhenDamaged");
            }
        }

        public static AbilityAsset LOG_WHEN_ATTACKED
        {
            get
            {
                return LoadAbilityAsset("LogWhenAttacked");
            }
        }

        public static AbilityAsset COUNTER_ATTACK
        {
            get
            {
                return LoadAbilityAsset("CounterAttack");
            }
        }

        public static AbilityAsset POISON
        {
            get
            {
                return LoadAbilityAsset("Poison");
            }
        }

        private static AbilityAsset LoadAbilityAsset(string fileName)
        {
            var asset = Resources.Load<AbilityAsset>(fileName);
            return asset;
        }

        private static MacroAsset ReadMacroAsset(string fileName)
        {
            var asset = Resources.Load<MacroAsset>(fileName);
            return asset;
        }
    }
}
