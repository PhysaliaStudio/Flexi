using System;

namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomExceptionNode : ProcessNode
    {
        protected override AbilityState DoLogic()
        {
            throw new Exception("This is for testing");
        }
    }
}
