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
    /// Interaction logic for MakeNewGroupchatWindow.xaml
    /// </summary>
    public partial class MakeNewGroupchatWindow : Window
    {

        public bool Result { get; private set; } = false;

        public MakeNewGroupchatWindow(List<string> usernames)
        {
            InitializeComponent();

            this.UsernameListView.ItemsSource = usernames;
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.GroupNameTextBox.Text)) this.Result = true;
            this.Close();
        }
    }
}
