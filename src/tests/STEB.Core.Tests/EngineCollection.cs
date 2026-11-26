using Xunit;

namespace STEB.Core.Tests;

/// <summary>
/// The 2010 engine keeps a few process-wide statics (DataBase.Tools.log, Progress counters) that are
/// not thread-safe. Tests that run engine calculations share this collection so they never run
/// concurrently with each other; they do not depend on each other's results (each builds its own input).
/// Pure parsing/geometry tests stay outside it and run in parallel.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class EngineCollection
{
    public const string Name = "Engine static state";
}
