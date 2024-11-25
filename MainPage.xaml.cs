using System.Runtime.CompilerServices;
using System;
using System.Collections.ObjectModel;
using seclab.Strategy;
using StudentsData;
using CommunityToolkit.Maui.Storage;

namespace seclab
{

    public partial class MainPage : ContentPage
    {
        private Binds.MainPageViewModel _viewModel;
        private static string _filePath;
        public string _selectedParser = "LINQ";
        private string SelectedParser { get { return _selectedParser; } }
        public static string FilePath { get => _filePath; set => _filePath = value; }

        public MainPage()
        {
            InitializeComponent();

            /* динамічно змінюємо значення атрибутів певних об'єктів на MainPage 
               P.S. якщо бути точним, то для FilePickerFrame змінюємо значення Grid.RowSpan та Grid.ColumnSpan */
            _viewModel = new Binds.MainPageViewModel();
            BindingContext = _viewModel;
        }

        public void InfoButtonHandler(object sender, EventArgs e)
        {
            Binds.InfoButtonHandler(this, sender, e);
        }

        public void ClearFiltersHandler(object sender, EventArgs e)
        {
            Binds.ClearFiltersHandler(this, sender, e, fullName: FullNameFilter, group: GroupFilter, grade: GradeFilter,
                                faculty: FacultyFilter, department: DepartmentFilter, specialization: SpecializationFilter);
        }

        public async void TransformXMLToHTML(object sender, EventArgs e)
        {
            Binds.TransformXMLToHTML(this, sender, e, FilePath);
        }

        public async void ResultButtonHandler(object sender, EventArgs e)
        {
            var fullName = FullNameFilter.Text;
            var group = GroupFilter.Text;
            var grade = GradeFilter.Text;
            var faculty = (FacultyFilter.SelectedItem as string);
            var departmment = (DepartmentFilter.SelectedItem as string);
            var specialization = (SpecializationFilter.SelectedItem as string);

            Binds.ResultButtonHandler(this, sender, e, technologyPicker: TechnologyPicker,
                fullName: fullName, group: group, grade: grade,
                faculty: faculty, department: departmment, specialization: specialization);
        }

        private async void OpenFileButtonHandler(object sender, EventArgs e)
        {
            List<Frame> Frames = new List<Frame> { TechnologiesFrame, FiltersFrame, ButtonsFrame };
            List<Picker> Pickers = new List<Picker> { FacultyFilter, DepartmentFilter, SpecializationFilter };
            Binds.OpenFileButtonHandler(this, sender, e, frames: Frames, firstFrame: FilePickerFrame, technologyPicker: TechnologyPicker, filePathLabel: FilePathLabel, _viewModel: this._viewModel, app: App.Current, pickers: Pickers);
        }
    }
}