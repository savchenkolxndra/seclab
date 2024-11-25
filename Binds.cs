using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using seclab.Strategy;
using Microsoft.Maui.Controls;
using StudentsData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace seclab
{
    public static class Binds
    {
        public static ObservableCollection<string> Technologies = new ObservableCollection<string> { "LINQ", "SAX", "DOM" };
        /// <summary>
        /// Днамічно змінюємо значення атрибутів Grid.RowSpan та Grid.ColumnSpan для FilePickerFrame
        /// </summary>
        public class MainPageViewModel : INotifyPropertyChanged
        {

            private int _rowSpan = 2; // Значення за замовчуванням
            private int _columnSpan = 2; // Значення за замовчуванням


            public int RowSpan { get => _rowSpan; set { _rowSpan = value; OnPropertyChanged(); } }

            public int ColumnSpan { get => _columnSpan; set { _columnSpan = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Змінюємо розмір вікна застосунку на певні фіксовані значення та змінюємо у FilePickerFrame значення атрибутів Grid.RowSpan та Grid.ColumnSpan
        /// </summary>
        private static void ChangeAppSize(MainPageViewModel _viewModel, Microsoft.Maui.Controls.Application app)
        {
            if (app != null)
            {
                var window = app.Windows[0];

                window.Width = window.MaximumWidth = window.MinimumWidth = 950;
                window.Height = window.MaximumHeight = window.MinimumHeight = 450;

                _viewModel.RowSpan = 1;
                _viewModel.ColumnSpan = 1;
            }
        }

        /// <summary>
        /// Робимо усі фрейми <paramref name="Widgets"/> видимими, а головний фрейм "підганяємо" по розміру сітки
        /// </summary>
        /// <param name="Widgets"></param>
        private static void MakeFramesVisible(List<Frame> Widgets, Frame firstFrame)
        {
            foreach (var Widget in Widgets)
            {
                Widget.IsVisible = true;
            }
        }

        public static void SetUniqueValues(string filePath, string node, string atribute, Picker picker)
        {
            var xmlDocument = XDocument.Load(MainPage.FilePath);

            var uniqueValues = xmlDocument.Descendants(node)
                    .Select(element => element.Attribute(atribute)?.Value) // Перевірка на null
                    .Where(value => !string.IsNullOrEmpty(value)) // Фільтрація null або порожніх значень
                    .Distinct()
                    .ToList();

            picker.Items.Add(" ");
            foreach (var item in uniqueValues)
            {
                picker.Items.Add(item);
            }
        }

        public static void InfoButtonHandler(MainPage mainPage, object sender, EventArgs e)
        {
            mainPage.DisplayAlert("Інформація про програму",
                $"Програма створена для зручної роботи з XML-файлами, зокрема для аналізу та трансформації даних. " +
                $"Ви можете обирати між трьома технологіями обробки XML: LINQ, SAX та DOM, " +
                $"\n\nВаріант інформаційної системи - \"Успішність студентів\"." +
                $"\n\nЦю роботу виконала Савченко Олександра, студентка групи К-26.",
                "ОК");
        }


        /// <summary>
        /// Обираємо *.xml файл і робимо видимими інші зони на сторінці />
        /// </summary>
        public static async void OpenFileButtonHandler(MainPage mainPage, object sender, EventArgs e,
            List<Frame> frames, Frame firstFrame, Label filePathLabel, Picker technologyPicker,
            MainPageViewModel _viewModel, Microsoft.Maui.Controls.Application app, List<Picker> pickers)
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[]{ ".xml" } }
            });

            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть необхідний .xml файл",
                FileTypes = customFileType
            });



            if (result != null)
            {
                MainPage.FilePath = result.FullPath;
                // Робимо фрейми видимими
                MakeFramesVisible(frames, firstFrame);

                // Змінюємо позиціювання FilePickerFrame
                ChangeAppSize(_viewModel, app);

                // Змінюємо текст FilePathLabel на шлях обраного файлу
                filePathLabel.Text = "Обрано: " + result.FullPath;
                filePathLabel.TextColor = Color.FromRgb(67, 130, 61);

                // Задаємо значення для пікера по обранню технології парсингу(LINQ, SAX, DOM)
                foreach (var item in Technologies)
                {
                    technologyPicker.Items.Add(item);
                }
                technologyPicker.SelectedItem = "LINQ";

                // Парсимо унікальні значення і задаємо їх для деких фільтрів-пікерів
                SetUniqueValues(MainPage.FilePath, "Student", "Faculty", pickers[0]);
                SetUniqueValues(MainPage.FilePath, "Student", "Department", pickers[1]);
                SetUniqueValues(MainPage.FilePath, "Student", "Specialization", pickers[2]);
            }
        }

        public static void ClearFiltersHandler(MainPage mainPage, object sender, EventArgs e, Entry fullName, Entry group, Entry grade,
                    Picker faculty, Picker department, Picker specialization)
        {
            // Очищення текстових полів
            fullName.Text = string.Empty;
            group.Text = string.Empty;
            grade.Text = string.Empty;
            // Скидання вибору в Picker
            faculty.SelectedIndex = -1;
            department.SelectedIndex = -1;
            specialization.SelectedIndex = -1;
        }


        public static async void ResultButtonHandler(MainPage mainPage, object sender, EventArgs e, Picker technologyPicker, string? fullName = null, string? group = null, string? grade = null,
            string? faculty = null, string? department = null, string? specialization = null)
        {
            if (mainPage == null) return;

            // Обираємо стратегію за результатом вибору користувачем відповідного пункту в Picker
            IStrategy selectedStrategy = (technologyPicker.SelectedItem as string) switch
            {
                "LINQ" => new LINQParser(),
                "SAX" => new SAXParser(),
                "DOM" => new DOMParser()
            };

            var studentsList = Filter.GetFilteredResult(selectedStrategy.Parse(MainPage.FilePath), fullName: fullName, group: group, grade: grade,
                faculty: faculty, department: department, specialization: specialization);
            ;
            ResultPage result = new ResultPage(MainPage.FilePath, studentsList);
            // Переходимо на сторінку з результатом
            await mainPage.Navigation.PushAsync(result);
        }


        public static async void TransformXMLToHTML(MainPage mainPage, object sender, EventArgs e, string xmlFilePath)
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xsl" } }
            });

            var xslResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть необхідний .xsl файл",
                FileTypes = customFileType
            });

            if (xslResult != null)
            {
                var xslFilePath = xslResult.FullPath;
                XslCompiledTransform xsl = new XslCompiledTransform();
                xsl.Load(xslFilePath);

                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                using var stream = new MemoryStream();
                try
                {
                    // Виконання трансформації та запис у MemoryStream
                    using (XmlReader reader = XmlReader.Create(xmlFilePath))
                    using (XmlWriter writer = XmlWriter.Create(stream, xsl.OutputSettings))
                    {
                        xsl.Transform(reader, writer);
                    }

                    // Скидання позиції потоку до початку для подальшого читання
                    stream.Position = 0;

                    // Збереження результату у файл за допомогою FileSaver
                    var fileSaverResult = await FileSaver.Default.SaveAsync("newfolder", "result.html", stream, cancellationToken.Token);
                    fileSaverResult.EnsureSuccess();

                    await Toast.Make($"Ви зберегли файл:\n{fileSaverResult.FilePath}").Show(cancellationToken.Token);
                }
                catch (Exception ex)
                {
                    await Toast.Make($"Ви не зберегли файл!").Show(cancellationToken.Token);
                }
            }
        }
    }
}
