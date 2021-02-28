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
using System.ComponentModel;

namespace Lab1_V2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public V2MainCollection main_collection { get; set; }
        public ICollectionView collection_view { get; set; }
        public ICollectionView grid_view { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewMenuItemClicked(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;

            main_collection = new V2MainCollection();

            collection_view = new CollectionViewSource() { Source = main_collection }.View;
            collection_view.Filter = CollectionFilter;

            grid_view = new CollectionViewSource() { Source = main_collection }.View;
            grid_view.Filter = GridFilter;

            this.DataContext = this;
        }

        private bool CollectionFilter(object item)
        {
            V2Data data = item as V2Data;
            return (data.GetType() == typeof(V2DataCollection));
        }

        private bool GridFilter(object item)
        {
            V2Data data = item as V2Data;
            return (data.GetType() == typeof(V2DataOnGrid));
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
