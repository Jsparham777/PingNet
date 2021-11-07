using Microsoft.Extensions.DependencyInjection;

namespace PingNet.ViewModels
{
    /// <summary>
    /// Locates the applicable viewmodel, from the IoC container, for a given view
    /// </summary>
    public class ViewModelLocator
    {      
        /// <summary>
        /// Gets the <see cref="MainWindowViewModel"/> from the IoC container
        /// </summary>
        public MainWindowViewModel MainWindowViewModel => App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
    }
}
