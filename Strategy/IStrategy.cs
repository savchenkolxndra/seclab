using StudentsData;

namespace seclab.Strategy
{
    public interface IStrategy
    {
        StudentsCollection Parse(string filePath);
    }
}

