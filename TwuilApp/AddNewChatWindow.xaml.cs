using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TwuilApp
{
    /// <summary>
    /// Interaction logic for AddNewChatWindow.xaml
    /// </summary>
    public partial class AddNewChatWindow : Window
    {

        public bool Result { get; private set; } = false;

        public AddNewChatWindow()
        {
            InitializeComponent();
        }

        private void AddNewChatButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.UserTextBox.Text)) this.Result = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}
