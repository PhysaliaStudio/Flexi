namespace Physalia.Flexi
{
    public interface IEventContext
    {

    }

    public interface IResumeContext
    {

    }

    public class EmptyEventContext : IEventContext
    {
        public static EmptyEventContext Instance { get; } = new EmptyEventContext();

        // Empty Content
    }

    public class EmptyResumeContext : IResumeContext
    {
        public static EmptyResumeContext Instance { get; } = new EmptyResumeContext();

        // Empty Content
    }
}
