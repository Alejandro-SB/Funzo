using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Funzo;

#if NET6_0_OR_GREATER

public struct ResultAwaiter<TResult, TOk, TErr> : ICriticalNotifyCompletion
    where TResult : ResultBase<TResult, TOk, TErr>, IResultBase<TResult, TOk, TErr>
{
    private readonly TResult _result;

    public ResultAwaiter(TResult result)
    {
        _result = result;
    }

    public bool IsCompleted => true;

    public TOk GetResult()
    {
        var isErr = _result.IsErr(out var ok, out var err);

        if (isErr)
        {
            ResultAsyncMethodBuilder<TResult, TOk, TErr>.SetError(err);
        }

        return !isErr ? ok : default;
    }

    public void OnCompleted(Action continuation) => continuation();
    public void UnsafeOnCompleted(Action continuation) => continuation();
}

public struct ResultAsyncMethodBuilder<TResult, TOk, TErr>
    where TResult : ResultBase<TResult, TOk, TErr>, IResultBase<TResult, TOk, TErr>
{
    private TResult _result;
    private Exception _exception;

    public static ResultAsyncMethodBuilder<TResult, TOk, TErr> Create() =>
        new();

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
        => stateMachine.MoveNext();

    public void SetResult(TOk result)
        => _result = TResult.Ok(result);

    public void SetException(Exception exception)
    {
        _exception = exception;
        _result = TResult.Err(default);
    }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        var completionAction = CreateCompletionAction(ref stateMachine);
        awaiter.OnCompleted(completionAction);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        var completionAction = CreateCompletionAction(ref stateMachine);
        awaiter.UnsafeOnCompleted(completionAction);
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine) { }

    public static void SetError(TErr err)
        => throw new ResultException(err);

    public TResult Task
    {
        get
        {
            if (_exception is ResultException resultException)
            {
                return TResult.Err(resultException.Err);
            }

            return _result;
        }
    }

    private Action CreateCompletionAction<TStateMachine>(
        ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        var boxedStateMachine = stateMachine;
        return boxedStateMachine.MoveNext;
    }

    private class ResultException : Exception
    {
        public TErr Err { get; }
        public ResultException(TErr err)
            : base("Result operation failed")
        {
            Err = err;
        }
    }
}
#endif