using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public static class CustomAbility
    {
        public static AbilityDataSource THROW_EXCEPTION
        {
            get
            {
                return LoadAbilityAsset("ThrowException");
            }
        }

        public static AbilityDataSource HELLO_WORLD
        {
            get
            {
                return LoadAbilityAsset("HelloWorld");
            }
        }

        public static AbilityDataSource HELLO_WORLD_MACRO_CALLER
        {
            get
            {
                return LoadAbilityAsset("HelloWorld_MacroCaller");
            }
        }

        public static AbilityDataSource HELLO_WORLD_MACRO_CALLER_5_TIMES
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

        public static AbilityDataSource HELLO_WORLD_MISSING_ELEMENTS
        {
            get
            {
                return LoadAbilityAsset("HelloWorld_MissingElements");
            }
        }

        public static AbilityDataSource NORAML_ATTACK
        {
            get
            {
                return LoadAbilityAsset("NormalAttack");
            }
        }

        public static AbilityDataSource NORAML_ATTACK_SELECTION
        {
            get
            {
                return LoadAbilityAsset("NormalAttackSelection");
            }
        }

        public static AbilityDataSource NORMAL_ATTACK_5_TIMES
        {
            get
            {
                return LoadAbilityAsset("NormalAttack5Times");
            }
        }

        public static AbilityDataSource ATTACK_DECREASE
        {
            get
            {
                return LoadAbilityAsset("AttackDecrease");
            }
        }

        public static AbilityDataSource ATTACK_DOUBLE
        {
            get
            {
                return LoadAbilityAsset("AttackDouble");
            }
        }

        public static AbilityDataSource ATTACK_UP_WHEN_LOW_HEALTH
        {
            get
            {
                return LoadAbilityAsset("AttackUpWhenLowHealth");
            }
        }

        public static AbilityDataSource ATTACK_DOUBLE_WHEN_DAMAGED
        {
            get
            {
                return LoadAbilityAsset("AttackDoubleWhenDamaged");
            }
        }

        public static AbilityDataSource ATTACK_DOUBLE_WHEN_GREATER_THAN_5
        {
            get
            {
                return LoadAbilityAsset("AttackDoubleWhenGreaterThan5");
            }
        }

        public static AbilityDataSource LOG_WHEN_ATTACKED
        {
            get
            {
                return LoadAbilityAsset("LogWhenAttacked");
            }
        }

        public static AbilityDataSource COUNTER_ATTACK
        {
            get
            {
                return LoadAbilityAsset("CounterAttack");
            }
        }

        public static AbilityDataSource POISON
        {
            get
            {
                return LoadAbilityAsset("Poison");
            }
        }

        private static AbilityDataSource LoadAbilityAsset(string fileName)
        {
            var asset = Resources.Load<AbilityAsset>(fileName);
            AbilityData data = asset.Data;
            AbilityDataSource dataSource = data.CreateDataSource(0);
            return dataSource;
        }

        private static MacroAsset ReadMacroAsset(string fileName)
        {
            var asset = Resources.Load<MacroAsset>(fileName);
            return asset;
        }
    }
}
