using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Funzo;
#if NET6_0_OR_GREATER
[AsyncMethodBuilder(typeof(ResultAsyncMethodBuilder<,,>))]
#endif
public class AwaitableResult<TResult, TOk, TErr>
    where TResult : ResultBase<TResult, TOk, TErr>, IResultBase<TResult, TOk, TErr>
{
    private readonly TResult _result;

    internal AwaitableResult(TResult result) => _result = result;

#if NET6_0_OR_GREATER
    public ResultAwaiter<TResult, TOk, TErr> GetAwaiter() => new(_result);
#endif
}
