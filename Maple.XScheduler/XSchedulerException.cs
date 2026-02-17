using System.Diagnostics.CodeAnalysis;

namespace Maple.XScheduler
{
    public class XSchedulerException(string? msg) : Exception(msg)
    {

        [DoesNotReturn]
        public static void Throw(string? msg) => throw new XSchedulerException(msg);

        [DoesNotReturn]
        public static T Throw<T>(string? msg) => throw new XSchedulerException(msg);

    }
}
