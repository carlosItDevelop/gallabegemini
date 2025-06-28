using System.Text.Json;

namespace GeneralLabSolutions.Domain.Extensions
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToLower();
        }
    }

}
