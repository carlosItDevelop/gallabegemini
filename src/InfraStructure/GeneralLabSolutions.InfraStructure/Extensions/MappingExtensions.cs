using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace GeneralLabSolutions.InfraStructure.Extensions
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Configura a conversão de um Enum para sua representação em string,
        /// respeitando o atributo [EnumMember(Value = "...")].
        /// </summary>
        public static PropertyBuilder<TEnum> HasEnumConversion<TEnum>(this PropertyBuilder<TEnum> propertyBuilder)
            where TEnum : struct, Enum
        {
            // O conversor que transforma o Enum em string para o banco
            var toProvider = new ValueConverter<TEnum, string>(
                v => v.ToEnumString(), // Usa nosso método customizado
                v => v.ToEnum<TEnum>() // Usa nosso método customizado
            );

            propertyBuilder.HasConversion(toProvider);
            propertyBuilder.HasColumnType("varchar(40)");

            return propertyBuilder;
        }

        // --- MÉTODOS HELPERS PRIVADOS ---

        // Converte um Enum para a string (lendo o atributo)
        private static string ToEnumString<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            var name = Enum.GetName(enumType, value);
            if (name == null)
                return string.Empty;

            var enumMemberAttribute = enumType.GetField(name)?
                .GetCustomAttributes(typeof(EnumMemberAttribute), true)
                .Cast<EnumMemberAttribute>()
                .FirstOrDefault();

            return enumMemberAttribute?.Value ?? name; // Se não tiver atributo, usa o nome do membro
        }

        // Converte uma string de volta para o Enum (procurando pelo atributo)
        private static TEnum ToEnum<TEnum>(this string value) where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = enumType.GetField(name)?
                    .GetCustomAttributes(typeof(EnumMemberAttribute), true)
                    .Cast<EnumMemberAttribute>()
                    .FirstOrDefault();

                if (enumMemberAttribute?.Value == value)
                {
                    return (TEnum)Enum.Parse(enumType, name);
                }
            }

            // Fallback para o nome do membro se não encontrar no atributo
            if (Enum.TryParse<TEnum>(value, true, out var result))
                return result;

            return default;
        }
    }
}