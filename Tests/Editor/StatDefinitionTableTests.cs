using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.Stats.Tests
{
    public class StatDefinitionTableTests
    {
        private readonly List<StatDefinition> validList = new()
        {
            new StatDefinition
            {
                Id = 1,
                Name = "Health"
            },
            new StatDefinition
            {
                Id = 2,
                Name = "MaxHealth"
            },
            new StatDefinition
            {
                Id = 11,
                Name = "Attack"
            },
        };

        private readonly List<StatDefinition> idConflictList = new()
        {
            new StatDefinition
            {
                Id = 1,
                Name = "Health"
            },
            new StatDefinition
            {
                Id = 2,
                Name = "MaxHealth"
            },
            new StatDefinition
            {
                Id = 2,
                Name = "Attack"
            },
        };

        [Test]
        public void CreateTable_WithValidList_ReturnsTableAsExpected()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(validList);
            for (var i = 0; i < validList.Count; i++)
            {
                Assert.AreSame(validList[i], table.GetStatDefinition(validList[i].Id));
            }
        }

        [Test]
        public void CreateTable_With1IdConfliction_ReturnsNullAndLog2Error()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(idConflictList);
            Assert.IsNull(table);
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            LogAssert.Expect(LogType.Error, new Regex(".*"));
        }

        [Test]
        public void GetDefinition_WithNonExistedId_ReturnsNullAndLogError()
        {
            var table = new StatDefinitionTable();
            Assert.AreSame(null, table.GetStatDefinition(999));
            LogAssert.Expect(LogType.Error, new Regex(".*"));
        }
    }
}
