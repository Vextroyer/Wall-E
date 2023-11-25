namespace GSharpCompiler;
///<summary>Represents errors occured during compilation or execution of a G# program.</summary>
public class Error
{
    ///<summary>Line where the error is found.</summary>
    public int Line { get; private set; }
    ///<summary>Aproimate column where the error is found.</summary>
    public int Offset { get; private set; }
    ///<summary>Error message</summary>
    public string Message { get; private set; }
    public Error(int line, int offset, string message)
    {
        Line = line;
        Offset = offset;
        Message = message;
    }
}