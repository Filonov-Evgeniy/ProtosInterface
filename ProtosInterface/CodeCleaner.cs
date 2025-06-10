using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace ProtosInterface
{
    class CodeCleaner
    {
        public static string ParseProductName(string fullName)
        {
            // Паттерны для выявления кодов изделий
            string pattern = @"
            (?<code>
                (?:\d{2,}(?:\.\d{2,}){1,}[A-ZА-Я]?\b)    # Коды типа 9511.19.020СБ
                |(?:ГОСТ\s?[A-ZА-Я]*-?\d+[A-ZА-Я]*-?\d*)  # ГОСТ/ISO стандарты
                |(?:ТУ\s?\d+-\d+-\d+)                     # Технические условия
                |(?:[A-ZА-Я]\d+-\d+[a-zа-я]*\w*)          # Коды типа М6-6gx20.46
            )";
            //(?:[A-ZА-Я]{1,2}\d{2,}[A-ZА-Я]?\b)       # Коды типа УЗ01К
            //|    
            var match = Regex.Match(fullName, pattern,
                RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string code = match.Groups["code"].Value;
                string shortName = fullName.Replace(code, "").Trim();

                // Очистка короткого названия от лишних символов
                shortName = Regex.Replace(shortName, @"[^\w\s-]", "").Trim();

                return shortName;
            }

            // Если код не найден, возвращаем исходную строку как название
            return fullName.Trim();
        }
    }
}
