using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funzo;

#if NET6_0_OR_GREATER

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
        try
        {
            return action();
        }
        catch (Exception e)
        {
            var exceptionType = e.GetType();

            foreach (var pair in _maps)
            {
                var type = pair.Key;
                var map = pair.Value;

                if (exceptionType == type)
                {
                    return TResult.Err(map(e));
                }
            }

            if(_else is { })
            {
                return TResult.Err(_else(e));
            }

            throw;
        }
    }

    public async Task<TResult> TryAsync(Func<Task<TResult>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception e)
        {
            var exceptionType = e.GetType();

            foreach (var pair in _maps)
            {
                var type = pair.Key;
                var map = pair.Value;

                if (exceptionType == type)
                {
                    return TResult.Err(map(e));
                }
            }

            if (_else is { })
            {
                return TResult.Err(_else(e));
            }

            throw;
        }
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
        try
        {
            return action();
        }
        catch (Exception e)
        {
            var exceptionType = e.GetType();

            foreach (var pair in _maps)
            {
                var type = pair.Key;
                var map = pair.Value;

                if (exceptionType == type)
                {
                    return TResult.Err(map(e));
                }
            }

            if (_else is { })
            {
                return TResult.Err(_else(e));
            }

            throw;
        }
    }

    public async Task<TResult> TryAsync(Func<Task<TResult>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception e)
        {
            var exceptionType = e.GetType();

            foreach (var pair in _maps)
            {
                var type = pair.Key;
                var map = pair.Value;

                if (exceptionType == type)
                {
                    return TResult.Err(map(e));
                }
            }

            if (_else is { })
            {
                return TResult.Err(_else(e));
            }

            throw;
        }
    }
}
#endif