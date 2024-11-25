using StudentsData;
using System.Xml.Linq;

namespace seclab.Strategy
{
    public class LINQParser : IStrategy
    {
        public StudentsCollection Parse(string filePath)
        {
            XDocument xml = XDocument.Load(filePath);
            var studentsList = new StudentsCollection { Students = new List<Student>() };

            foreach (var studentElement in xml.Descendants("Student"))
            {
                var student = new Student
                {
                    FullName = (string)studentElement.Attribute("FullName"),
                    Faculty = (string)studentElement.Attribute("Faculty"),
                    Department = (string)studentElement.Attribute("Department"),
                    Specialization = (string)studentElement.Attribute("Specialization"),
                    Group = (string)studentElement.Attribute("Group"),
                    Disciplines = studentElement.Descendants("Discipline").Select(s => new Discipline
                    {
                        Name = (string)s.Attribute("Name"),
                        Grade = (string)s.Attribute("Grade")
                    }).ToList()
                };
                studentsList.Students.Add(student);
            }

            return studentsList;
        }
    }
}
