using System;

namespace Physalia.Flexi.Tests
{
    public class CustomExceptionNode : ProcessNode
    {
        protected override AbilityState DoLogic()
        {
            throw new Exception("This is for testing");
        }
    }
}
