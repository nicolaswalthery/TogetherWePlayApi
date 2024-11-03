namespace Common.Exceptions
{
    public  class SafeExecuteException : Exception
    {

        public SafeExecuteException(string message, string reason, string? methodNameExecuted = null, Exception? innerException = null) : base(message, innerException)
        {
            Reason = reason;
            MethodNameExecuted = methodNameExecuted;
        }

        public string Reason { get; set; }
        public string? MethodNameExecuted { get; }

        public string? GetStackTrace()
        {
            return base.StackTrace;
        }
    }
}
