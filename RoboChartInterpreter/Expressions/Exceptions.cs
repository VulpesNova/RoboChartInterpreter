using System.Diagnostics.CodeAnalysis;

namespace RoboChartInterpreter.Expressions;

[ExcludeFromCodeCoverage]
public class ExpressionInterpreterException : Exception
{
    public ExpressionInterpreterException() : base() { }
    public ExpressionInterpreterException(string? message) : base(message) { }
    public ExpressionInterpreterException(string? message, Exception? innerException) : base(message, innerException) { }
}

[ExcludeFromCodeCoverage]
public class InvalidOperationRCException : ExpressionInterpreterException
{
    public InvalidOperationRCException() : base() { }
    public InvalidOperationRCException(string? message) : base(message) { }
    public InvalidOperationRCException(string? message, Exception? innerException) : base(message, innerException) { }
}

[ExcludeFromCodeCoverage]
public class InvalidTypeRCException : ExpressionInterpreterException
{
    public InvalidTypeRCException() : base() { }
    public InvalidTypeRCException(string? message) : base(message) { }
    public InvalidTypeRCException(string? message, Exception? innerException) : base(message, innerException) { }
}