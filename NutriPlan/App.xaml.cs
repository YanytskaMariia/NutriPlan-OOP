using NutriPlan.Presentation.ViewModels;
using NutriPlan.Presentation.Views;
using System.Windows;

namespace NutriPlan
{
    public partial class App : System.Windows.Application
    {
        private MainWindow MainWindow;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            base.Shutdown();
            // Clean up resources if needed
        }
    }
}