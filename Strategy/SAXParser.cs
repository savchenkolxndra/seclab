using StudentsData;
using System.Xml;

namespace seclab.Strategy
{
    public class SAXParser : IStrategy
    {
        public StudentsCollection Parse(string filePath)
        {
            var studentsCollection = new StudentsCollection();
            Student currentStudent = null;

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "Student":
                                currentStudent = new Student
                                {
                                    FullName = reader.GetAttribute("FullName"),
                                    Faculty = reader.GetAttribute("Faculty"),
                                    Department = reader.GetAttribute("Department"),
                                    Specialization = reader.GetAttribute("Specialization"),
                                    Group = reader.GetAttribute("Group"),
                                    Disciplines = new List<Discipline>()
                                };
                                break;

                            case "Discipline":
                                var discipline = new Discipline
                                {
                                    Name = reader.GetAttribute("Name"),
                                    Grade = reader.GetAttribute("Grade")
                                };
                                currentStudent?.Disciplines.Add(discipline);
                                break;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Student")
                    {
                        if (currentStudent != null)
                        {
                            studentsCollection.Students.Add(currentStudent);
                            currentStudent = null;
                        }
                    }
                }
            }

            return studentsCollection;
        }
    }
}
