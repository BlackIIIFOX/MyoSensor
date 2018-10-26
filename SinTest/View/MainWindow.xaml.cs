using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyoSensor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainWindowModel = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mainWindowModel;
        }

        private void MenuProfiles_Click(object sender, RoutedEventArgs e)
        {
            MenuItem obMenuItem = e.OriginalSource as MenuItem;
            this.FullName.Header = obMenuItem.Header.ToString();
            mainWindowModel.SelectedProfile = this.FullName.Header.ToString();
            // MessageBox.Show(String.Format("{0} just said Hi!", obMenuItem.Header));
        }

        private void MenuSession_Click(object sender, RoutedEventArgs e)
        {
            MenuItem obMenuItem = e.OriginalSource as MenuItem;
            mainWindowModel.SelectedSession = obMenuItem.Header.ToString();
        }

    }
}
