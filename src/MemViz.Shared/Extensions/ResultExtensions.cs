using MemViz.Shared.Results;

namespace MemViz.Shared.Extensions;

/// <summary>
/// Provides extension methods for the Result type to enable functional programming patterns
/// such as mapping and binding operations for both synchronous and asynchronous scenarios.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Transforms the value of a successful Result using the provided mapper function.
    /// If the Result is a failure, returns a new failure Result with the same error.
    /// </summary>
    /// <typeparam name="TIn">The type of the input Result value.</typeparam>
    /// <typeparam name="TOut">The type of the output Result value.</typeparam>
    /// <param name="result">The Result to transform.</param>
    /// <param name="mapper">A function that transforms the input value to the output value.</param>
    /// <returns>A new Result containing the transformed value if successful, or the original error if failed.</returns>
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result.Success(mapper(result.Value!))
            : Result.Failure<TOut>(result.Error);
    }
    
    /// <summary>
    /// Asynchronously transforms the value of a successful Result using the provided async mapper function.
    /// If the Result is a failure, returns a new failure Result with the same error.
    /// </summary>
    /// <typeparam name="TIn">The type of the input Result value.</typeparam>
    /// <typeparam name="TOut">The type of the output Result value.</typeparam>
    /// <param name="result">The Result to transform.</param>
    /// <param name="mapper">An async function that transforms the input value to the output value.</param>
    /// <returns>A Task containing a new Result with the transformed value if successful, or the original error if failed.</returns>
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<TOut>> mapper)
    {
        return result.IsSuccess
            ? Result.Success(await mapper(result.Value!))
            : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    /// Chains two Result operations together. If the input Result is successful, applies the binder function
    /// which returns a new Result. If the input Result is a failure, returns a new failure Result with the same error.
    /// </summary>
    /// <typeparam name="TIn">The type of the input Result value.</typeparam>
    /// <typeparam name="TOut">The type of the output Result value.</typeparam>
    /// <param name="result">The Result to bind.</param>
    /// <param name="binder">A function that takes the input value and returns a new Result.</param>
    /// <returns>The Result returned by the binder function if input is successful, or the original error if failed.</returns>
    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> binder)
    {
        return result.IsSuccess
            ? binder(result.Value!)
            : Result.Failure<TOut>(result.Error);
    }
    
    /// <summary>
    /// Asynchronously chains two Result operations together. If the input Result is successful, applies the async binder function
    /// which returns a new Result. If the input Result is a failure, returns a new failure Result with the same error.
    /// </summary>
    /// <typeparam name="TIn">The type of the input Result value.</typeparam>
    /// <typeparam name="TOut">The type of the output Result value.</typeparam>
    /// <param name="result">The Result to bind.</param>
    /// <param name="binder">An async function that takes the input value and returns a new Result.</param>
    /// <returns>A Task containing the Result returned by the binder function if input is successful, or the original error if failed.</returns>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> binder)
    {
        return result.IsSuccess
            ? await binder(result.Value!)
            : Result.Failure<TOut>(result.Error);
    }
}