using NUnit.Framework;

namespace Physalia.AbilitySystem.Tests
{
    public class StatsRefreshWithModifierTests
    {
        private StatOwner CreateOwner()
        {
            StatDefinitionListAsset statDefinitionList = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            StatOwnerRepository repository = StatOwnerRepository.Create(statDefinitionList);
            StatOwner owner = repository.CreateOwner();
            owner.AddStat(StatTestHelper.HEALTH, 100);
            owner.AddStat(StatTestHelper.MAX_HEALTH, 100);
            owner.AddStat(StatTestHelper.ATTACK, 12);
            return owner;
        }

        [Test]
        public void SingleModifier_WithStatIdOwnerNotOwned_NoError()
        {
            var effect = new AbilityContext { ContextType = AbilityContext.Type.MODIFIER };
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.MUL,
                Value = 50,
            });

            var instance = new AbilityContextInstance(effect);
            var owner = CreateOwner();
            owner.RemoveStat(StatTestHelper.ATTACK);

            owner.AppendAbilityContext(instance);
            owner.RefreshStats();

            Assert.IsNull(owner.GetStat(StatTestHelper.ATTACK));
            Assert.Pass();
        }

        [Test]
        public void SingleModifier_WithInvalidStatId_NoError()
        {
            var effect = new AbilityContext { ContextType = AbilityContext.Type.MODIFIER };
            effect.Effects.Add(new AbilityEffect
            {
                StatId = 999,
                Op = AbilityEffect.Operator.MUL,
                Value = 50,
            });

            var instance = new AbilityContextInstance(effect);
            var owner = CreateOwner();

            owner.AppendAbilityContext(instance);
            owner.RefreshStats();

            Assert.Pass();
        }

        [Test]
        public void SingleModifier_100Minus10_CurrentBaseReturns100AndCurrentValueReturns90()
        {
            var effect = new AbilityContext { ContextType = AbilityContext.Type.MODIFIER };
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.MAX_HEALTH,
                Op = AbilityEffect.Operator.ADD,
                Value = -10,
            });

            var instance = new AbilityContextInstance(effect);
            var owner = CreateOwner();

            owner.AppendAbilityContext(instance);
            owner.RefreshStats();

            Assert.AreEqual(100, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
        }

        [Test]
        public void SingleModifier_12Mul50Percent_CurrentBaseReturns12AndCurrentValueReturns18()
        {
            var effect = new AbilityContext { ContextType = AbilityContext.Type.MODIFIER };
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.MUL,
                Value = 50,
            });

            var instance = new AbilityContextInstance(effect);
            var owner = CreateOwner();

            owner.AppendAbilityContext(instance);
            owner.RefreshStats();

            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(18, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_Base12Add6AndMul50Percent_CurrentBaseReturns12AndCurrentValueReturns27()
        {
            var effect = new AbilityContext { ContextType = AbilityContext.Type.MODIFIER };
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.MUL,
                Value = 50,
            });
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.ADD,
                Value = 6,
            });

            var instance = new AbilityContextInstance(effect);
            var owner = CreateOwner();

            owner.AppendAbilityContext(instance);
            owner.RefreshStats();

            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_WithDifferentStatIds()
        {
            var effect = new AbilityContext { ContextType = AbilityContext.Type.MODIFIER };
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.MUL,
                Value = 50,
            });
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.ADD,
                Value = 6,
            });
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.MAX_HEALTH,
                Op = AbilityEffect.Operator.ADD,
                Value = -10,
            });

            var instance = new AbilityContextInstance(effect);
            var owner = CreateOwner();

            owner.AppendAbilityContext(instance);
            owner.RefreshStats();

            Assert.AreEqual(100, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void RefreshTwice_ReturnsTheSameValues()
        {
            var effect = new AbilityContext { ContextType = AbilityContext.Type.MODIFIER };
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.MUL,
                Value = 50,
            });
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.ADD,
                Value = 6,
            });
            effect.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.MAX_HEALTH,
                Op = AbilityEffect.Operator.ADD,
                Value = -10,
            });

            var instance = new AbilityContextInstance(effect);
            var owner = CreateOwner();

            owner.AppendAbilityContext(instance);
            owner.RefreshStats();
            owner.RefreshStats();

            Assert.AreEqual(100, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }
    }
}
