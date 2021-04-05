using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MyLibrary;

namespace Lab2_V2_UI
{
    public class DataItemClass: IDataErrorInfo, INotifyPropertyChanged
    {
        public Vector2 grid_coord;
        public float grid_coord_x
        {
            get
            {
                return grid_coord.X;
            }
            set
            {
                grid_coord.X = value;
                OnPropertyChanged("grid_coord_x");
            }
        }
        public float grid_coord_y
        {
            get
            {
                return grid_coord.Y;
            }
            set
            {
                grid_coord.Y = value;
                OnPropertyChanged("grid_coord_y");
            }
        }

        public Vector2 EM_value;
        public float EM_value_real
        {
            get
            {
                return EM_value.X;
            }
            set
            {
                EM_value.X = value;
                OnPropertyChanged("EM_value_real");
            }
        }
        public float EM_value_imaginary
        {
            get
            {
                return EM_value.Y;
            }
            set
            {
                EM_value.Y = value;
                OnPropertyChanged("EM_value_imaginary");
            }
        }

        public V2DataCollection data_colletcion;
        public string Error { get { return "Error Text"; } }
        public string this[string property]
        {
            get
            {
                string msg = null;

                switch (property)
                {
                    case "EM_value_real":
                        if ((EM_value.X == 0) && (EM_value.Y == 0))
                            msg = "EM value magnitude can't be equal to 0";
                        break;
                    case "EM_value_imaginary":
                        if ((EM_value.X == 0) && (EM_value.Y == 0))
                            msg = "EM value magnitude can't be equal to 0";
                        break;
                    default:
                        break;
                }
                return msg;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public DataItemClass()
        {
        }
        public DataItemClass(V2DataCollection dataCollection)
        {
            data_colletcion = dataCollection;
        }

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
