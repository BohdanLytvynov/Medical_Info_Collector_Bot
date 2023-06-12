using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemViewModels
{
    public static class Validation
    {

        static public char[] Restricted { get => restricted; }

        static char[] restricted = { ' ', '.', ',', '/', '\\', '|', '#', '@', '!', '?', '!', '&', '^', ':', ';','%','&','*','(',')'
                , '+', '=', '[', ']','{','}','"', '>', '<', '~'};
        public static bool ValidateText(string txt, char[] symbols, out string error, bool ShouldFieldBeEmpty = false)
        {
            if (String.IsNullOrEmpty(txt) && !ShouldFieldBeEmpty)
            {
                error = "Поле не має бути порожнім!";

                return false;
            }

            foreach (var item in symbols)
            {
                if (txt.Contains(item))
                {
                    error = $"Текстове поле не має містити символу: [ {item} ] !";

                    return false;
                }
            }

            for (int i = 0; i < txt.Length; i++)
            {
                if (Char.IsDigit(txt[i]))
                {
                    error = $"Текстове поле містить цифру під індексом: {i + 1}!";

                    return false;
                }
            }

            error = "";

            return true;
        }

        public static bool ValidateCode(string code, out string error)
        {
            if (String.IsNullOrEmpty(code))
            {
                error = "Поле не має бути порожнім!";

                return false;
            }

            var ar = code.Split('-');

            if (ar.Length != 4)
            {
                error = $"Неправильна кількість цифр коду!";

                return false;
            }

            for (int i = 0; i < ar.Length; i++)
            {
                for (int j = 0; j < ar[i].Length; j++)
                {
                    if (!Char.IsDigit(ar[i][j]))
                    {
                        error = $"{i + 1} группа цифр містить невірний символ під індексом {j + 1}!!";

                        return false;
                    }
                }
            }

            for (int i = 0; i < ar.Length; i++)
            {
                if (ar[i].Length != 4)
                {
                    error = $"Невірна кількість цифр в {i + 1} группі";

                    return false;
                }
            }

            error = "";

            return true;
        }

        public static bool ValidateDateTime(string date, out string error)
        {
            error = String.Empty;

            if (String.IsNullOrWhiteSpace(date))
            {
                error = "Пусте поле";

                return false;
            }

            DateTime dtemp;

            if (DateTime.TryParse(date, out dtemp))
            {
                return true;
            }
            else
            {
                error = "Невірно заповнено поле!!!";

                return false;
            }
        }

        public static bool ValidateDateTime(DateTime date, out string error)
        {
            error = String.Empty;

            DateTime dtemp;

            if (DateTime.TryParse(date.ToString(), out dtemp))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public static bool ValidateNumber(string number, out string error, bool ShouldFieldBeEmpty = false)
        {

            if (String.IsNullOrWhiteSpace(number) && !ShouldFieldBeEmpty)
            {
                error = "Поле не має бути порожнім!";

                return false;
            }

            for (int i = 0; i < number.Length; i++)
            {
                if (!Char.IsDigit(number[i]))
                {
                    error = $"Невірний символ під індексом {i + 1}";

                    return false;
                }
            }

            error = String.Empty;

            return true;
        }
    }
}
