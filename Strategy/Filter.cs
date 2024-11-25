using StudentsData;
using System.Text.RegularExpressions;

namespace seclab.Strategy
{
    public static class Filter
    {
        /// <summary>
        /// Розбиває вхідний рядок на слова та розділові знаки.
        /// </summary>
        private static string[] SplitIntoWords(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Array.Empty<string>();

            var regex = new Regex(@"[\w'-]+|[^\w\s]+");
            return regex.Matches(input).Select(m => m.Value).ToArray();
        }

        /// <summary>
        /// Перевіряє, чи є частковий збіг між фільтром користувача та значенням атрибуту.
        /// </summary>
        private static bool IsSimilarParts(string? userFilter, string? attributeValue)
        {
            var filterParts = SplitIntoWords(userFilter?.ToLower());
            var attributeParts = SplitIntoWords(attributeValue?.ToLower());

            return filterParts.Any(filter =>
                attributeParts.Any(attribute => attribute.StartsWith(filter)));
        }

        /// <summary>
        /// Фільтрує список студентів за заданими критеріями.
        /// </summary>
        public static StudentsCollection GetFilteredResult(
            StudentsCollection studentsList,
            string? fullName = null,
            string? group = null,
            string? grade = null,
            string? faculty = null,
            string? department = null,
            string? specialization = null)
        {
            // Перевірка, чи хоча б один фільтр заданий
            if (new[] { fullName, group, grade, faculty, department, specialization }.All(string.IsNullOrWhiteSpace))
            {
                return studentsList;
            }

            var filteredStudents = studentsList.Students.Where(student =>
                MatchesFilter(faculty, student.Faculty) &&
                MatchesFilter(department, student.Department) &&
                MatchesFilter(specialization, student.Specialization) &&
                MatchesFilter(fullName, student.FullName) &&
                MatchesFilter(group, student.Group) &&
                MatchesGrade(grade, student)
            ).ToList();

            return new StudentsCollection { Students = filteredStudents };
        }

        /// <summary>
        /// Перевіряє, чи атрибут відповідає заданому фільтру.
        /// </summary>
        private static bool MatchesFilter(string? filter, string? attributeValue)
        {
            return string.IsNullOrWhiteSpace(filter) || IsSimilarParts(filter, attributeValue);
        }

        /// <summary>
        /// Перевіряє, чи студент має відповідну оцінку.
        /// </summary>
        private static bool MatchesGrade(string? grade, Student student)
        {
            return string.IsNullOrWhiteSpace(grade) ||
                   student.Disciplines.Any(discipline => discipline.Grade == grade);
        }
    }
}
