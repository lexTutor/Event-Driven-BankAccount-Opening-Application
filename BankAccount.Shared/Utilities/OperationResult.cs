namespace BankAccount.Shared.Utilities
{
    /// <summary>
    /// Represents the result of an operation.
    /// </summary>
    public class OperationResult<TResult>
    {
        public TResult Result { get; set; }

        /// <summary>
        /// An <see cref="IEnumerable{String}"/> containing an errors that occurred during the operation.
        /// </summary>
        public IEnumerable<string> Errors { get; protected set; }

        /// <summary>
        /// Whether if the operation succeeded or not.
        /// </summary>
        public bool Successful { get; protected set; }

        /// <summary>
        /// Creates an <see cref="OperationResult"/> indicating a failed operation, with a list of errors if applicable.
        /// </summary>
        /// <param name="errors">An optional array of <see cref="string"/> which caused the operation to fail.</param>
        public static OperationResult<TResult> Failed(params string[] errors) => new OperationResult<TResult> { Successful = false, Errors = errors };

        /// <summary>
        /// Returns an <see cref="OperationResult"/>indicating a successful operation.
        /// </summary>
        public static OperationResult<TResult> Success { get; } = new OperationResult<TResult> { Successful = true };
    }
}
