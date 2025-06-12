namespace Common.ResultPattern
{
    public static class Safe
    {
        public static async Task<Result<T>> ExecuteAsync<T>(Func<Task<Result<T>>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                return Result<T>.Failure($"Exception: {ex.Message}", ReasonType.Unexpected);
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
