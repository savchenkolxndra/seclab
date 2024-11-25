using StudentsData;
using System.Xml;

namespace seclab.Strategy
{
    public class DOMParser : IStrategy
    {
        public StudentsCollection Parse(string filePath)
        {
            // Створюємо об'єкт для збереження колекції студентів
            var studentsCollection = new StudentsCollection();

            // Завантажуємо XML-документ
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Знаходимо всі вузли <Student>
            var studentNodes = xmlDoc.SelectNodes("//Student");

            if (studentNodes != null)
            {
                foreach (XmlNode studentNode in studentNodes)
                {
                    // Створюємо об'єкт студента
                    var student = new Student
                    {
                        FullName = studentNode.Attributes?["FullName"]?.InnerText,
                        Faculty = studentNode.Attributes?["Faculty"]?.InnerText,
                        Department = studentNode.Attributes?["Department"]?.InnerText,
                        Specialization = studentNode.Attributes?["Specialization"]?.InnerText,
                        Group = studentNode.Attributes?["Group"]?.InnerText,
                        Disciplines = new List<Discipline>()
                    };

                    // Перебираємо всі дочірні вузли <Discipline>
                    foreach (XmlNode childNode in studentNode.ChildNodes)
                    {
                        if (childNode.Name == "Discipline")
                        {
                            // Створюємо об'єкт дисципліни
                            var discipline = new Discipline
                            {
                                Name = childNode.Attributes?["Name"]?.InnerText,
                                Grade = childNode.Attributes?["Grade"]?.InnerText
                            };

                            // Додаємо дисципліну до списку дисциплін студента
                            student.Disciplines.Add(discipline);
                        }
                    }

                    // Додаємо студента до колекції
                    studentsCollection.Students.Add(student);
                }
            }

            return studentsCollection;
        }
    }
}
