namespace GeneralLabSolutions.Domain.Extensions.Extending
{
    public static class StringUtils
    {
        public static string ApenasNumeros(this string input) // 'input' agora é a string estendida
        {
            if (string.IsNullOrEmpty(input)) // Boa prática: tratar nulo/vazio
            {
                return string.Empty;
            }
            return new string(input.Where(char.IsDigit).ToArray());
        }
    }

}
