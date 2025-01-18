using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class ActorStatsTests
    {
        private AbilitySystem abilitySystem;

        [SetUp]
        public void SetUp()
        {
            AbilitySystemBuilder builder = new AbilitySystemBuilder();
            abilitySystem = builder.Build();
        }

        private static AbilityData CreateAbilityDataWith1Group()
        {
            var abilityData = new AbilityData();
            abilityData.graphGroups.Add(new AbilityGraphGroup());
            return abilityData;
        }

        [Test]
        public void SetStat_OriginalBaseIs2AndNewBaseIs6_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            Actor actor = new EmptyActor(abilitySystem);

            actor.AddStat(CustomStats.ATTACK, 2);
            actor.GetStat(CustomStats.ATTACK).CurrentBase = 6;
            actor.ApplyModifiers();

            Stat stat = actor.GetStat(CustomStats.ATTACK);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }

        [Test]
        public void ModifyStat_OriginalBaseIs2AndAdd4_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            Actor actor = new EmptyActor(abilitySystem);

            actor.AddStat(CustomStats.ATTACK, 2);
            actor.GetStat(CustomStats.ATTACK).CurrentBase += 4;
            actor.ApplyModifiers();

            Stat stat = actor.GetStat(CustomStats.ATTACK);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }

        [Test]
        public void AppendAbilityDataContainer_OriginalContainersCountIs0_ContainersCountReturns1()
        {
            AbilityData abilityData = CreateAbilityDataWith1Group();
            var container = new AbilityDataContainer { DataSource = abilityData.CreateDataSource(0) };

            Actor actor = new EmptyActor(abilitySystem);
            actor.AppendAbilityDataContainer(container);

            Assert.AreEqual(1, actor.AbilityDataContainers.Count);
        }

        [Test]
        public void RemoveAbilityDataContainer_OriginalContainersCountIs1_ContainersCountReturns0()
        {
            AbilityData abilityData = CreateAbilityDataWith1Group();
            var container = new AbilityDataContainer { DataSource = abilityData.CreateDataSource(0) };

            Actor actor = new EmptyActor(abilitySystem);
            actor.AppendAbilityDataContainer(container);
            actor.RemoveAbilityDataContainer(container);

            Assert.AreEqual(0, actor.AbilityDataContainers.Count);
        }

        [Test]
        public void ClearAllAbilityDataContainers_OriginalContainersCountIs2_ContainersCountReturns0()
        {
            AbilityData abilityData = CreateAbilityDataWith1Group();
            var container1 = new AbilityDataContainer { DataSource = abilityData.CreateDataSource(0) };
            var container2 = new AbilityDataContainer { DataSource = abilityData.CreateDataSource(0) };

            Actor actor = new EmptyActor(abilitySystem);
            actor.AppendAbilityDataContainer(container1);
            actor.AppendAbilityDataContainer(container2);
            actor.ClearAllAbilityDataContainers();

            Assert.AreEqual(0, actor.AbilityDataContainers.Count);
        }

        [Test]
        public void AppendAbilityDataContainer_ContainerActorReturnsOwner()
        {
            AbilityData abilityData = CreateAbilityDataWith1Group();
            var container = new AbilityDataContainer { DataSource = abilityData.CreateDataSource(0) };

            Actor actor = new EmptyActor(abilitySystem);
            actor.AppendAbilityDataContainer(container);

            Assert.AreEqual(actor, container.Actor);
        }

        [Test]
        public void RemoveAbilityDataContainer_ContainerActorReturnsNull()
        {
            AbilityData abilityData = CreateAbilityDataWith1Group();
            var container = new AbilityDataContainer { DataSource = abilityData.CreateDataSource(0) };

            Actor actor = new EmptyActor(abilitySystem);
            actor.AppendAbilityDataContainer(container);
            actor.RemoveAbilityDataContainer(container);

            Assert.AreEqual(null, container.Actor);
        }

        [Test]
        public void ClearAllAbilityDataContainers_EachContainerActorReturnsNull()
        {
            AbilityData abilityData = CreateAbilityDataWith1Group();
            var container1 = new AbilityDataContainer { DataSource = abilityData.CreateDataSource(0) };
            var container2 = new AbilityDataContainer { DataSource = abilityData.CreateDataSource(0) };

            Actor actor = new EmptyActor(abilitySystem);
            actor.AppendAbilityDataContainer(container1);
            actor.AppendAbilityDataContainer(container2);
            actor.ClearAllAbilityDataContainers();

            Assert.AreEqual(null, container1.Actor);
            Assert.AreEqual(null, container2.Actor);
        }
    }
}
