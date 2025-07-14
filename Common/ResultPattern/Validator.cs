using Common.Extensions;

namespace Common.ResultPattern
{
    public static class Validator
    {
        /// <summary>
        /// Main method of the Validator. Its function is to verify data according to the 'checking method' its given.
        /// </summary>
        /// <typeparam name="T">Data Type.</typeparam>
        /// <param name="data">Data to be verified.</param>
        /// <param name="validators">List of methods that validate the data.</param>
        /// <returns>[SUCCESS] - Return the data in a Result object. [FAILURE] - Return the first Result object where its state is 'IsFailure == true'.</returns>
        public static Result<T> Verify<T>(this T data, params Func<T, Result<T>>[] validators)
            where T : class
        {
            foreach (var validator in validators)
            {
                var result = validator(data);
                if (result.IsFailure)
                    return result;
            }

            return Result<T>.Success(data);
        }

        /// <summary>
        /// Validate that the data that we want to retreive from the database has been found.
        /// </summary>
        /// <typeparam name="T">Type of the data retrieved from the database.</typeparam>
        /// <param name="data">Data found in the database.</param>
        /// <param name="error">Error message that can be personalize if the validation failed.</param>
        /// <returns>[SUCCESS] - Return the data in a Result object. [FAILURE] - Return a Result object where its state is 'IsFailure == true'.</returns>
        public static Result<T> IsNull<T> (this T data, string error = "")
            where T : class
        {
            if (data is null)
                return Result<T>.Failure(error.IsNullOrEmptyOrWhiteSpace() ? $"{typeof(T)} - No data found !" : $"{nameof(T)} - {error}", ReasonType.NotFound);
            return Result<T>.Success(data);
        }

        /// <summary>
        /// Validate that the data that we want to retreive from the database has been found.
        /// </summary>
        /// <typeparam name="T">Type of the data retrieved from the database.</typeparam>
        /// <param name="data">Data found in the database.</param>
        /// <param name="error">Error message that can be personalize if the validation failed.</param>
        /// <returns>[SUCCESS] - Return the data in a Result object. [FAILURE] - Return a Result object where its state is 'IsFailure == true'.</returns>
        public static Result<IEnumerable<T>> HasNoElement<T>(this Result<IEnumerable<T>> result, string error = "")
        {
            if (result.Data is null || !result.Data.Any())
                return Result<IEnumerable<T>>.Failure(error.IsNullOrEmptyOrWhiteSpace() ? $"{nameof(T)} - No data found!" : $"{nameof(T)} - {error}", ReasonType.NotFound);

            return Result<IEnumerable<T>>.Success(result.Data);
        }
    }
}
