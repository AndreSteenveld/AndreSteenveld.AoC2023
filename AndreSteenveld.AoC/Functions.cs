namespace AndreSteenveld.AoC;

using System.Diagnostics;
using System.Numerics;

public static partial class Functions {

    // These were taken from wikipedia and stack overflow
    public static long LeastCommonMultiple(long a, long b) => Math.Abs(a * b) / GreatestCommonDivisor(a, b);
    public static long GreatestCommonDivisor(long a, long b) => b is 0L ? a : GreatestCommonDivisor(b, a % b);

    public static IEnumerable<TNumber> Range<TNumber>(TNumber start, TNumber count) where TNumber : INumber<TNumber>, INumberBase<TNumber>{

        for(TNumber current = TNumber.Zero; current < count; ++current)
            yield return start + current;

    }

    public static T[] CreateArray<T>( int capacity, params T[] values ){
        System.Array.Resize(ref values, capacity);
        return values;
    }

     public static void WaitForDebugger(){
        if(Environment.GetEnvironmentVariable("DOTNET_WAIT_FOR_DEBUGGER") == "1"){
            Console.WriteLine("Waiting for debugger to attach...");
            
            while(Debugger.IsAttached is false)
                Thread.Sleep(500);

        }
    }


}