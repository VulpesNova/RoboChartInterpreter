namespace RoboChartInterpreter.Expressions;

class ExpressionInterpreterException : Exception
{
    public ExpressionInterpreterException() : base() { }
    public ExpressionInterpreterException(string? message) : base(message) { }
    public ExpressionInterpreterException(string? message, Exception? innerException) : base(message, innerException) { }
}

class InvalidOperationRCException : ExpressionInterpreterException
{
    public InvalidOperationRCException() : base() { }
    public InvalidOperationRCException(string? message) : base(message) { }
    public InvalidOperationRCException(string? message, Exception? innerException) : base(message, innerException) { }
}

class InvalidTypeRCException : ExpressionInterpreterException
{
    public InvalidTypeRCException() : base() { }
    public InvalidTypeRCException(string? message) : base(message) { }
    public InvalidTypeRCException(string? message, Exception? innerException) : base(message, innerException) { }
}