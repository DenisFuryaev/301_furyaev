using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Collections.Specialized;

using MyLibrary;

namespace Lab1_V2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public V2MainCollection main_collection { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewMenuItemClicked(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            //main_collection = null;    
            
            main_collection = new V2MainCollection();
            this.DataContext = this;
        }

        private void AddDefaultsClicked(object sender, RoutedEventArgs e)
        {
            main_collection.AddDefaults();
        }

        private void AddDefaultV2DataCollectionClicked(object sender, RoutedEventArgs e)
        {
            main_collection.AddDefaultV2DataCollection();
        }

        private void AddDefaultV2DataOnGridClicked(object sender, RoutedEventArgs e)
        {
            main_collection.AddDefaultV2DataOnGrid();
        }

        private void RemoveClicked(object sender, RoutedEventArgs e)
        {
            int selected_index = ListBox_Main.SelectedIndex;
            main_collection.Remove(selected_index);
        }
    }
}
