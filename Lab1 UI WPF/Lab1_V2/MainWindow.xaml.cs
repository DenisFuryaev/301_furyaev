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

        private V2MainCollection main_collection;

        private void CollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs args)
        {
            //Handle collection changed event
            TextBox.Text = "debug text";
        }

        public MainWindow()
        {
            InitializeComponent();


            main_collection = new V2MainCollection();
            this.DataContext = main_collection;
            main_collection.CollectionChanged += CollectionChangedEventHandler;

            ListBox_Main.DataContext = DataContext;
        }

        private void NewMenuItemClicked(object sender, RoutedEventArgs e)
        {
            main_collection = new V2MainCollection();
        }

        private void AddDefaultsClicked(object sender, RoutedEventArgs e)
        {
            main_collection.AddDefaults();

            //foreach (V2Data data in main_collection)
            //    ListBox_Main.Items.Add(data);
        }

        private void AddDefaultV2DataCollectionClicked(object sender, RoutedEventArgs e)
        {
            main_collection.AddDefaultV2DataCollection();
        }
    }
}
