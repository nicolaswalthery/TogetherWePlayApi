using Common.Extensions;

namespace Common.ResultPattern
{
    public static class ValidateHelpers
    {
        /// <summary>
        /// Validate if the data that we want to retreive from the database has been found.
        /// </summary>
        /// <typeparam name="T">Type of the data retrieved from the database.</typeparam>
        /// <param name="data">Data found in the database.</param>
        /// <param name="error">Error message that can be personalize if the validation failed.</param>
        /// <returns>[SUCCESS] - Return the data in a Result object. [FAILURE] - Return a Result object where its state is 'IsFailure == true'.</returns>
        public static Result<T> IsNull<T> (this T data, string error = "")
            where T : class
        {
            if (data is null)
                return Result<T>.Failure(error.IsNullOrEmptyOrWhiteSpace() ? $"{nameof(T)} - No data found !" : $"{nameof(T)} - {error}", ReasonType.NotFound);
            return Result<T>.Success(data);
        }
    }
}
