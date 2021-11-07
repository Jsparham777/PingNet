using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PingNet.ViewModels
{
    /// <summary>
    /// Base View Model containing the <see cref="INotifyPropertyChanged"/> implementation
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Instantiates a new Base View Model object, exposing the ApplicationSettings Model and contains the INotifyPropertyChanged implementation. This inherits from all View Models
        /// </summary>
        public BaseViewModel()
        {
        }

        #region Property Changed event handler
        /// <summary>
        /// The property change event is triggered when a property is updated. This is used by WPF to update the GUI
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The property change method, called when a property value is updated. This triggers the PropertyChangedEventHandler to notify the WPF GUI an update needs to happen.
        /// </summary>
        /// <param name="propName">The name of the class Property</param>
        public void NotifyPropertyChanged([CallerMemberName]string propName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        /// <summary>
        /// Raises the property changed method on all properties in the class by passing null to the PropertyChangedEventArgs
        /// </summary>
        public void RaiseAllPropertiesChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        #endregion
    }
}
