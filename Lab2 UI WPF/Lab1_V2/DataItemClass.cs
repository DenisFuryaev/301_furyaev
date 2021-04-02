using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Windows.Input;
using MyLibrary;

namespace Lab2_V2_UI
{
    public class DataItemClass: IDataErrorInfo
    {
        public Vector2 grid_coord { get; set; }
        public Complex EM_value { get; set; }
        public string Error { get { return "Error Text"; } }
        public string this[string property]
        {
            get
            {
                string msg = null;
                switch (property)
                {
                    case "grid_coord":
                        break;
                    case "EM_value":
                        break;
                    default:
                        break;
                }
                return msg;
            }
        }
        public DataItemClass(V2DataCollection dataCollection)
        {
            DataItem dataItem = new DataItem(grid_coord, EM_value);
            dataCollection.Add(dataItem);
        }

    }
}
