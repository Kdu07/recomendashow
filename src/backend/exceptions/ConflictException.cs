namespace BackEnd.Aventura.Exceptions
{
    public sealed class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
