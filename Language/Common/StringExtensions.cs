using System.Text.RegularExpressions;

namespace Language.JwtExtensions;

public static class StringExtensions
{
    public static List<string> SplitToWords(this string? text)
    {
        if(string.IsNullOrWhiteSpace(text))
        {
            return new List<string>();
        }

        // Удаляем все символы, кроме букв, цифр и пробелов
        string cleanedText = Regex.Replace(text, @"[^\p{L}\p{N}\s]", "");

        // Заменяем множественные пробелы на одинарные
        cleanedText = Regex.Replace(cleanedText, @"\s+", " ").Trim();

        // Разделяем текст на слова (учитываем разные пробельные символы)
        List<string> words = cleanedText.Split(new[] { ' ', '\t', '\n', '\r' }, 
                StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        return words;
    }


}