using seclab.Strategy;
using StudentsData;

namespace seclab;

public partial class ResultPage : ContentPage
{
    public StudentsCollection Students { get; set; }

    public ResultPage(string filePath, StudentsCollection result)
    {
        InitializeComponent();
        Students = result;
        BindingContext = Students;
    }
}