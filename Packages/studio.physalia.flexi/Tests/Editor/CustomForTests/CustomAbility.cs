using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public static class CustomAbility
    {
        public static AbilityData THROW_EXCEPTION
        {
            get
            {
                return LoadAbilityData("ThrowException");
            }
        }

        public static AbilityData HELLO_WORLD
        {
            get
            {
                return LoadAbilityData("HelloWorld");
            }
        }

        public static AbilityData HELLO_WORLD_MACRO_CALLER
        {
            get
            {
                return LoadAbilityData("HelloWorld_MacroCaller");
            }
        }

        public static AbilityData HELLO_WORLD_MACRO_CALLER_5_TIMES
        {
            get
            {
                return LoadAbilityData("HelloWorld_MacroCaller5Times");
            }
        }

        public static MacroAsset HELLO_WORLD_MACRO
        {
            get
            {
                return ReadMacroAsset("HelloWorld_Macro");
            }
        }

        public static AbilityData HELLO_WORLD_MISSING_ELEMENTS
        {
            get
            {
                return LoadAbilityData("HelloWorld_MissingElements");
            }
        }

        public static AbilityData NORAML_ATTACK
        {
            get
            {
                return LoadAbilityData("NormalAttack");
            }
        }

        public static AbilityData NORAML_ATTACK_SELECTION
        {
            get
            {
                return LoadAbilityData("NormalAttackSelection");
            }
        }

        public static AbilityData NORMAL_ATTACK_5_TIMES
        {
            get
            {
                return LoadAbilityData("NormalAttack5Times");
            }
        }

        public static AbilityData ATTACK_DECREASE
        {
            get
            {
                return LoadAbilityData("AttackDecrease");
            }
        }

        public static AbilityData ATTACK_DOUBLE
        {
            get
            {
                return LoadAbilityData("AttackDouble");
            }
        }

        public static AbilityData ATTACK_UP_WHEN_LOW_HEALTH
        {
            get
            {
                return LoadAbilityData("AttackUpWhenLowHealth");
            }
        }

        public static AbilityData ATTACK_DOUBLE_WHEN_DAMAGED
        {
            get
            {
                return LoadAbilityData("AttackDoubleWhenDamaged");
            }
        }

        public static AbilityData ATTACK_DOUBLE_WHEN_GREATER_THAN_5
        {
            get
            {
                return LoadAbilityData("AttackDoubleWhenGreaterThan5");
            }
        }

        public static AbilityData LOG_WHEN_ATTACKED
        {
            get
            {
                return LoadAbilityData("LogWhenAttacked");
            }
        }

        public static AbilityData COUNTER_ATTACK
        {
            get
            {
                return LoadAbilityData("CounterAttack");
            }
        }

        public static AbilityData POISON
        {
            get
            {
                return LoadAbilityData("Poison");
            }
        }

        private static AbilityData LoadAbilityData(string fileName)
        {
            var asset = Resources.Load<AbilityAsset>(fileName);
            return asset.Data;
        }

        private static MacroAsset ReadMacroAsset(string fileName)
        {
            var asset = Resources.Load<MacroAsset>(fileName);
            return asset;
        }
    }
}
