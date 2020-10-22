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
using TwuilApp.Data;

namespace TwuilApp
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        public ChatWindow()
        {
            InitializeComponent();

            List<DChatItem> chatItemList = new List<DChatItem>();
            chatItemList.Add(new DChatItem
            {
                ChatName = "Test",
                LastMessage = "Henk is een steen"
            });
            chatItemList.Add(new DChatItem
            {
                ChatName = "Test",
                LastMessage = "Henk is een steen"
            });
            chatItemList.Add(new DChatItem
            {
                ChatName = "Test",
                LastMessage = "Henk is een steen"
            });
            chatItemList.Add(new DChatItem
            {
                ChatName = "Test",
                LastMessage = "Henk is een steen"
            });

            this.ChatItemControl.ItemsSource = chatItemList;

            chatItemList.Add(new DChatItem
            {
                ChatName = "Guilliam",
                LastMessage = "Guilliam is een held"
            });
        }
    }
}
