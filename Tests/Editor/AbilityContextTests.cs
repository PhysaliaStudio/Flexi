using NUnit.Framework;

namespace Physalia.AbilitySystem.Tests
{
    public class AbilityContextTests
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
        public void ActionToModifyStat_TheOwnerDoesNotHaveTheStat_NoError()
        {
            var context = new AbilityContext();
            context.Effects.Add(new AbilityEffect
            {
                StatId = 999,
                Op = AbilityEffect.Operator.ADD,
                Value = -4,
            });

            var instance = new AbilityContextInstance(context);
            StatOwner owner = CreateOwner();
            instance.Calculate(owner);

            Assert.Pass();
        }

        [Test]
        public void ActionToModifyStat_Health100Minus4_CurrentValueReturns96()
        {
            var context = new AbilityContext();
            context.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.HEALTH,
                Op = AbilityEffect.Operator.ADD,
                Value = -4,
            });

            var instance = new AbilityContextInstance(context);
            StatOwner owner = CreateOwner();
            instance.Calculate(owner);

            Assert.AreEqual(96, owner.GetStat(StatTestHelper.HEALTH).CurrentValue);
        }

        [Test]
        public void ActionToModifyStat_Health100Minus4Twice_CurrentValueReturns92()
        {
            var context = new AbilityContext();
            context.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.HEALTH,
                Op = AbilityEffect.Operator.ADD,
                Value = -4,
            });

            var instance = new AbilityContextInstance(context);
            StatOwner owner = CreateOwner();
            instance.Calculate(owner);
            instance.Calculate(owner);

            Assert.AreEqual(92, owner.GetStat(StatTestHelper.HEALTH).CurrentValue);
        }

        [Test]
        public void ActionToModifyStat_Attack12Mul50Percent_CurrentValueReturns18()
        {
            var context = new AbilityContext();
            context.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.MUL,
                Value = 50,
            });

            var instance = new AbilityContextInstance(context);
            StatOwner owner = CreateOwner();
            instance.Calculate(owner);

            Assert.AreEqual(18, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void ActionToModifyStat_Attack12Set30_CurrentValueReturns30()
        {
            var context = new AbilityContext();
            context.Effects.Add(new AbilityEffect
            {
                StatId = StatTestHelper.ATTACK,
                Op = AbilityEffect.Operator.SET,
                Value = 30,
            });

            var instance = new AbilityContextInstance(context);
            StatOwner owner = CreateOwner();
            instance.Calculate(owner);

            Assert.AreEqual(30, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void ExecuteModifierEffect_CurrentBaseIs12_CurrentBaseReturns12AndCurrentValueReturns12()
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
            instance.Calculate(owner);

            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }
    }
}
