using Castle.DynamicProxy;
using System.Diagnostics;

namespace XAM.Interceptors;

public class TimeTakenInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            invocation.Proceed();
        }
        finally
        {
            stopwatch.Stop();
            
            Console.WriteLine($"Method `{invocation.Method.Name}` took {stopwatch.ElapsedMilliseconds} ms to execute.");
        }
    }
}
