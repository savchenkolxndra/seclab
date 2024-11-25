namespace StudentsData
{
    // Клас Студенти
    public class StudentsCollection
    {
        public List<Student> Students { get; set; }
        public StudentsCollection()
        {
            Students = new List<Student>();
        }
    }

    // Клас Студент
    public class Student
    {
        public string FullName { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
        public string Specialization { get; set; }
        public string Group { get; set; }
        public List<Discipline> Disciplines { get; set; }
    }

    // Клас дисципліна
    public class Discipline
    {
        public string Name { get; set; }
        public string Grade { get; set; }
    }
}

