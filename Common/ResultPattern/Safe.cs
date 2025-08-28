using Common.Exceptions;
using System.Reflection;

namespace Common.ResultPattern
{
    public static class Safe
    {
        private static string _reason = "An unmanaged exception occur and has been catch during the safe execution.";
        public static async Task<Result<T>> ExecuteAsync<T>(Func<Task<Result<T>>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                return Result<T>.Failure($"Exception: {action.GetMethodInfo().Name} - {ex.Message}", ReasonType.Unexpected);
            }
        }

        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                throw new SafeExecuteException(ex.Message, action.Method.Name, _reason, e.InnerException);
            }
        }

        public static async Task<Result> ExecuteAsync(Func<Task<Result>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Exception: {ex.Message}", ReasonType.Unexpected);
            }
        }

        public static Result<T> Execute<T>(Func<Result<T>> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                return Result<T>.Failure($"Exception: {ex.Message}", ReasonType.Unexpected);
            }
        }

        public static Result Execute(Func<Result> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Exception: {ex.Message}", ReasonType.Unexpected);
            }
        }
    }
}
