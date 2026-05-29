namespace BackEnd.Aventura.Domain
{
    public static class EntityGuards
    {
        public static Guid NotEmpty(Guid value, string paramName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Identificador inválido.", paramName);
            }

            return value;
        }

        public static string NotNullOrWhiteSpace(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} é obrigatório.", paramName);
            }

            return value.Trim();
        }
    }
}
