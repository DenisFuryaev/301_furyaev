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
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace Lab1_V2
{
    public class CoordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Vector2 coord = (Vector2)value;
            return "coord = " + coord.ToString("F3");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Complex em_value = (Complex)value;
            return "value = " + em_value.ToString("F3");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class MaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is V2DataOnGrid item)
                return item.GetMax().ToString("F2");
            else
                return "null";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class MinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is V2DataOnGrid item)
                return item.GetMin().ToString("F2");
            else
                return "null";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class IsModifiedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool item = (bool)value;
            if (item)
                return "not saved";
            else
                return "saved";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public partial class MainWindow : Window
    {
        public V2MainCollection main_collection { get; set; }
        public ICollectionView main_view { get; set; }
        public ICollectionView collection_view { get; set; }
        public ICollectionView grid_view { get; set; }
        public V2DataCollection selected_items { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
            main_collection = new V2MainCollection();
            UpdateBindings();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            AlertIfMofified();
        }

        private void AlertIfMofified()
        {
            MessageBoxResult result;
            if (main_collection != null && main_collection.IsModified)
            {
                result = MessageBox.Show("You have modified collection but didnt saved it.\n Do you wont to save it?", "Save or not?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveCommandHandler(null, null);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
        private void UpdateBindings()
        {
            this.DataContext = null;

            main_view = new CollectionViewSource() { Source = main_collection }.View;
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


        private void OpenCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            AlertIfMofified();
            Microsoft.Win32.OpenFileDialog open_dialoge = new Microsoft.Win32.OpenFileDialog();
            open_dialoge.ShowDialog();
            string filename = open_dialoge.FileName;
            if (!string.IsNullOrEmpty(filename))
                main_collection.Load(filename);

            UpdateBindings();
        }
        private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog save_dialoge = new Microsoft.Win32.SaveFileDialog();
            save_dialoge.ShowDialog();
            string filename = save_dialoge.FileName;
            if (!string.IsNullOrEmpty(filename))
                main_collection.Save(filename);
        }
        private void SaveCommandHandler_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((main_collection == null) || (!main_collection.IsModified))
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }
        private void NewCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (main_collection != null)
                AlertIfMofified();
            main_collection = new V2MainCollection();
            UpdateBindings();
        }
        private void RemoveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            main_collection.Remove(ListBox_Main.SelectedIndex);
        }
        private void RemoveCommandHandler_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((ListBox_Main == null) || (ListBox_Main.SelectedIndex == -1))
                e.CanExecute = false;
            else
                e.CanExecute = true;
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
        private void AddElementFromFileClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog open_dialoge = new Microsoft.Win32.OpenFileDialog();
            open_dialoge.ShowDialog();
            string filename = open_dialoge.FileName;
            if (!string.IsNullOrEmpty(filename))
                main_collection.AddV2DataOnGridFromFile(filename);

            UpdateBindings();
        }


        private void AddDataItemClicked(object sender, RoutedEventArgs e)
        {

        }

        private void AddDataItemCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void AddDataItemCommandHandler_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }
    }

    public static class CustomCommands
    {
        //public static RoutedCommand AddDataItem = new RoutedCommand("Add DataItem", typeof(Lab1_V2.MainWindow));

        public static readonly RoutedUICommand AddDataItem = new RoutedUICommand
            (
                "Add DataItem",
                "Add DataItem",
                typeof(Lab1_V2.MainWindow),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.A, ModifierKeys.Control)
                }
            );


    }
}