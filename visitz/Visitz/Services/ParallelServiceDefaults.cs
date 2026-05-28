namespace Visitz.Services;

#nullable enable

internal static class ParallelServiceDefaults
{
    /// <summary>
    /// Default maximum degree of parallelism for parallelized API services. Since the main parallelization of those
    /// services are spent waiting for IO rather than actual work, we should be able to arbitrarily set this value high
    /// without much performance penalty.
    /// </summary>
    public static int MaxParallelism = Environment.ProcessorCount * 100;
}
