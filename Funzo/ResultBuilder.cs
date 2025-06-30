using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Funzo;

public class ResultBuilder<TResult, TOk, TErr> : IResultBuilder<TResult, TOk, TErr>
    where TResult : IResultBase<TResult, TOk, TErr>
{
    private readonly Dictionary<Type, Func<object, TErr>> _maps = new();
    private Func<Exception, TErr>? _else;

    public ResultBuilder() { }

    public IResultBuilder<TResult, TOk, TErr> MapElse(Func<Exception, TErr> map)
    {
        _else = map;

        return this;
    }

    public IResultBuilder<TResult, TOk, TErr> MapException<TException>(Func<TException, TErr> map) where TException : Exception
    {
        _maps[typeof(TException)] = new Func<object, TErr>(x => map((TException)x));

        return this;
    }

    public TResult Try(Func<TResult> action)
    {
        return BuilderHelpers.Try(action, _maps, _else, Producer);
    }

    public async Task<TResult> TryAsync(Func<Task<TResult>> action)
    {
        return await BuilderHelpers.TryAsync(action, _maps, _else, Producer);
    }

    private TResult Producer(TErr err)
    {
#if NET6_0_OR_GREATER
            return TResult.Err(err);
#else
        var staticErr = typeof(TResult).GetMethod("Err", BindingFlags.Static | BindingFlags.Public);
        return (TResult)staticErr.Invoke(null, new object[] { err! });
#endif
    }
}


public class ResultBuilder<TResult, TErr> : IResultBuilder<TResult, TErr>
    where TResult : IResultBase<TResult, TErr>
{
    private readonly Dictionary<Type, Func<object, TErr>> _maps = new();
    private Func<Exception, TErr>? _else;

    public ResultBuilder() { }

    public IResultBuilder<TResult, TErr> MapElse(Func<Exception, TErr> map)
    {
        _else = map;

        return this;
    }

    public IResultBuilder<TResult, TErr> MapException<TException>(Func<TException, TErr> map) where TException : Exception
    {
        _maps[typeof(TException)] = new Func<object, TErr>(x => map((TException)x));

        return this;
    }

    public TResult Try(Func<TResult> action)
    {
        return BuilderHelpers.Try(action, _maps, _else, Producer);
    }

    public async Task<TResult> TryAsync(Func<Task<TResult>> action)
    {
        return await BuilderHelpers.TryAsync(action, _maps, _else, Producer);
    }

    private TResult Producer(TErr err)
    {
#if NET6_0_OR_GREATER
            return TResult.Err(err);
#else
        var staticErr = typeof(TResult).GetMethod("Err", BindingFlags.Static | BindingFlags.Public);
        return (TResult)staticErr.Invoke(null, new object[] { err! });
#endif
    }
}

internal class BuilderHelpers
{
    internal static TResult Try<TResult, TErr>(Func<TResult> action, Dictionary<Type, Func<object, TErr>> maps, Func<Exception, TErr>? otherwise, Func<TErr, TResult> producer)
    {
        try
        {
            return action();
        }
        catch (Exception e)
        {
            var result = ManageException(e, maps, otherwise, producer);

            if (result is null)
            {
                throw;
            }

            return result;
        }
    }

    internal async static Task<TResult> TryAsync<TResult, TErr>(Func<Task<TResult>> action, Dictionary<Type, Func<object, TErr>> maps, Func<Exception, TErr>? otherwise, Func<TErr, TResult> producer)
    {
        try
        {
            return await action();
        }
        catch (Exception e)
        {
            var result = ManageException(e, maps, otherwise, producer);

            if (result is null)
            {
                throw;
            }

            return result;
        }
    }

    private static TResult? ManageException<TResult, TErr>(Exception e, Dictionary<Type, Func<object, TErr>> maps, Func<Exception, TErr>? otherwise, Func<TErr, TResult> producer)
    {
        var exceptionType = e.GetType();

        foreach (var pair in maps)
        {
            var type = pair.Key;
            var map = pair.Value;

            if (exceptionType == type)
            {
                var result = map(e)!;
                return producer(result);
            }
        }

        if (otherwise is { })
        {
            var result = otherwise(e)!;

            return producer(result);
        }

        return default;
    }
}