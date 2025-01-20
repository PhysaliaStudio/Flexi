using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public static class CustomAbility
    {
        public static AbilityHandle THROW_EXCEPTION
        {
            get
            {
                return LoadAbilityAsset("ThrowException");
            }
        }

        public static AbilityHandle HELLO_WORLD
        {
            get
            {
                return LoadAbilityAsset("HelloWorld");
            }
        }

        public static AbilityHandle HELLO_WORLD_MACRO_CALLER
        {
            get
            {
                return LoadAbilityAsset("HelloWorld_MacroCaller");
            }
        }

        public static AbilityHandle HELLO_WORLD_MACRO_CALLER_5_TIMES
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

        public static AbilityHandle HELLO_WORLD_MISSING_ELEMENTS
        {
            get
            {
                return LoadAbilityAsset("HelloWorld_MissingElements");
            }
        }

        public static AbilityHandle NORAML_ATTACK
        {
            get
            {
                return LoadAbilityAsset("NormalAttack");
            }
        }

        public static AbilityHandle NORAML_ATTACK_SELECTION
        {
            get
            {
                return LoadAbilityAsset("NormalAttackSelection");
            }
        }

        public static AbilityHandle NORMAL_ATTACK_5_TIMES
        {
            get
            {
                return LoadAbilityAsset("NormalAttack5Times");
            }
        }

        public static AbilityHandle ATTACK_DECREASE
        {
            get
            {
                return LoadAbilityAsset("AttackDecrease");
            }
        }

        public static AbilityHandle ATTACK_DOUBLE
        {
            get
            {
                return LoadAbilityAsset("AttackDouble");
            }
        }

        public static AbilityHandle ATTACK_UP_WHEN_LOW_HEALTH
        {
            get
            {
                return LoadAbilityAsset("AttackUpWhenLowHealth");
            }
        }

        public static AbilityHandle ATTACK_DOUBLE_WHEN_DAMAGED
        {
            get
            {
                return LoadAbilityAsset("AttackDoubleWhenDamaged");
            }
        }

        public static AbilityHandle ATTACK_DOUBLE_WHEN_GREATER_THAN_5
        {
            get
            {
                return LoadAbilityAsset("AttackDoubleWhenGreaterThan5");
            }
        }

        public static AbilityHandle LOG_WHEN_ATTACKED
        {
            get
            {
                return LoadAbilityAsset("LogWhenAttacked");
            }
        }

        public static AbilityHandle COUNTER_ATTACK
        {
            get
            {
                return LoadAbilityAsset("CounterAttack");
            }
        }

        public static AbilityHandle POISON
        {
            get
            {
                return LoadAbilityAsset("Poison");
            }
        }

        private static AbilityHandle LoadAbilityAsset(string fileName)
        {
            var asset = Resources.Load<AbilityAsset>(fileName);
            AbilityData data = asset.Data;
            AbilityHandle handle = data.CreateHandle(0);
            return handle;
        }

        private static MacroAsset ReadMacroAsset(string fileName)
        {
            var asset = Resources.Load<MacroAsset>(fileName);
            return asset;
        }
    }
}
