namespace seclab
{
    public partial class App : Application
    {
        private int appWidth = 475;
        private int appHeight = 295;

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }
        protected override Window CreateWindow(IActivationState activationState)
        {

            Window window = base.CreateWindow(activationState);
            if (window != null)
            {
                window.Title = "Робота з .xml файлами";
                window.Width = window.MaximumWidth = window.MinimumWidth = appWidth;
                window.Height = window.MaximumHeight = window.MinimumHeight = appHeight;
            }
#if WINDOWS
            window.Created += (s, e) =>
            {
                var handle = WinRT.Interop.WindowNative.GetWindowHandle(window.Handler.PlatformView);
                var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

                //and here it is
                appWindow.Closing += async (s, e) =>
                {
                    e.Cancel = true;
                    bool result = await App.Current.MainPage.DisplayAlert(
                        "Попередження",
                        "Ви впевнені, що хочете вийти?",
                        "Так",
                        "Ні");

                    if (result)
                    {
                        App.Current.Quit();
                    }
                };
            };
#endif
            return window;
        }
    }
}
