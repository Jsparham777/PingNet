using System.Windows;

namespace PingNet.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : Window
    {
        /// <summary>
        /// Gets the dialog caption.
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Gets the OK button text.
        /// </summary>
        public string OkButtonText { get; private set; }

        /// <summary>
        /// Gets the cancel button text.
        /// </summary>
        public string CancelButtonText { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [show cancel button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show cancel button]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCancelButton { get; private set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogBox"/> class.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="caption">The dialog caption.</param>
        /// <param name="okOnly">if set to <c>true</c> [OK button only].</param>
        /// <param name="okButtonText">The OK button text.</param>
        /// <param name="cancelButtonText">The cancel button text.</param>
        public DialogBox(string title, string caption, bool okOnly = false, string okButtonText = "Yes", string cancelButtonText = "No")
        {
            InitializeComponent();

            DataContext = this;

            Title = title;
            Caption = caption;
            CancelButtonText = cancelButtonText;
            
            if (okOnly)
            {
                ShowCancelButton = false;
                OkButtonText = "Ok";
            }
            else
            {
                OkButtonText = okButtonText;    
            }
        }

        /// <summary>
        /// Handles the Click event of the OK button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of the cancel button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
