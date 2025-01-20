using System;

namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomExceptionNode : DefaultProcessNode
    {
        protected override AbilityState DoLogic()
        {
            throw new Exception("This is for testing");
        }
    }
}
