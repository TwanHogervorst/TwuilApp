using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for AddNewGroupchatWindow.xaml
    /// </summary>
    public partial class AddNewGroupchatWindow : Window
    {

        public AddNewGroupchatWindow(List<string> usernameList)
        {
            InitializeComponent();

            this.UsernameListView.ItemsSource = usernameList;
        }

        private void AddUsersButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.UsernameListView.SelectedItems.Count == 0)
            {
                MessageBoxResult result = MessageBox.Show("Please selected minimal 1 username.", "Select user", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (result == MessageBoxResult.Cancel) this.Close();
            } 
            else
            {
                this.Close();
            }
        }
    }
}
