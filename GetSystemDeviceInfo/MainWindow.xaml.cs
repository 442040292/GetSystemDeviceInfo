using iBot.Core.Utils;
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

namespace GetSystemDeviceInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            comboBox.ItemsSource = Enum.GetValues(typeof(MachineInfoHelper.DeviceInforType)).Cast<MachineInfoHelper.DeviceInforType>().ToList(); ;
            comboBox.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //"Win32_Processor"
            //"Win32_BaseBoard"
            textBox.Text = MachineInfoHelper.GetDiviceInfo("Win32_Processor", false);
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = MachineInfoHelper.GetDiviceInfo("Win32_BaseBoard", false);
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = MachineInfoHelper.GetDiviceInfo("Win32_BIOS", false);

        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = MachineInfoHelper.GetDiviceInfo(comboBox.Text, false);
        }
    }
}
