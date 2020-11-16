using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace FirstProject
{

    // grid item type
    struct DataItem
    {
        public Vector2 grid_coord { get; set; }
        public Complex EM_value;

        public DataItem(Vector2 grid_coord, Complex EM_value)
        {
            this.grid_coord = grid_coord;
            this.EM_value = EM_value;
        }

        public override string ToString()
        {
            return $"coordinate - ({grid_coord.X}, {grid_coord.Y}); value = {EM_value}\n";
        }

        public string ToString(string format)
        {
            string output = "coordinates = " + grid_coord.ToString(format) + "; ";
            output += "value = " + EM_value.ToString(format) + "; ";
            output += "magnitude = " + EM_value.Magnitude.ToString(format) + ";\n";
            return output;
        }
    }

    // grid settings
    struct Grid1D
    {
        public float stride { get; set; }
        public int knot_count { get; set; }

        public Grid1D(float stride, int knot_count)
        {
            this.stride = stride;
            this.knot_count = knot_count;
        }

        public override string ToString()
        {
            return $"stride = {stride}; knot_count = {knot_count}\n";
        }

        public string ToString(string format)
        {
            string output = "stride = " + stride.ToString(format) + "; ";
            output += "knot_count = " + knot_count.ToString(format) + ";\n";
            return output;
        }
    }

    // abstract class
    abstract class V2Data
    {
        public string info { get; set; }
        public double EM_frequency { get; set; }

        public V2Data(string info, double EM_frequency)
        {
            this.info = info;
            this.EM_frequency = EM_frequency;
        }

        public abstract Complex[] NearAverage(float eps);
        public abstract string ToLongString();

        public override string ToString()
        {
            return $"info = {info}; EM_frequency = {EM_frequency}\n";
        }
    }

    class V2DataOnGrid : V2Data
    {
        public Grid1D[] grid_settings { get; set; }
        Complex[,] EM_array;

        public V2DataOnGrid(string info, double EM_frequency, Grid1D OX_settings, Grid1D OY_settings) : base(info, EM_frequency)
        {
            EM_array = new Complex[OX_settings.knot_count, OY_settings.knot_count];
            grid_settings = new Grid1D[2];
            grid_settings[0] = OX_settings;
            grid_settings[1] = OY_settings;
        }

        public void InitRandom(double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                double tmp = maxValue;
                maxValue = minValue;
                minValue = tmp;
            }

            Random rnd = new Random();
            for (int j = 0; j < grid_settings[1].knot_count; j++)
            {
                for (int i = 0; i < grid_settings[0].knot_count; i++)
                {
                    EM_array[i, j] = new Complex(minValue + (maxValue - minValue) * rnd.NextDouble(), minValue + (maxValue - minValue) * rnd.NextDouble());
                }
            }
        }

        public static explicit operator V2DataCollection(V2DataOnGrid data_on_grid)
        {
            V2DataCollection data_collection = new V2DataCollection(data_on_grid.info, data_on_grid.EM_frequency);
            DataItem data_item;
            Vector2 coord = new Vector2(0.0f, 0.0f);
            float x_stride = data_on_grid.grid_settings[0].stride, y_stride = data_on_grid.grid_settings[1].stride;

            for (int j = 0; j < data_on_grid.grid_settings[1].knot_count; j++)
            {
                for (int i = 0; i < data_on_grid.grid_settings[0].knot_count; i++)
                {
                    data_item = new DataItem(coord, data_on_grid.EM_array[i, j]);
                    data_collection.EM_list.Add(data_item);
                    coord.X += x_stride;
                }
                coord.Y += y_stride;
                coord.X = 0;
            }

            return data_collection;
        }

        public override Complex[] NearAverage(float eps)
        {
            double mean_real = 0, sum_real = 0;
            int x_knot_count = grid_settings[0].knot_count, y_knot_count = grid_settings[1].knot_count;
            Complex[] output_array = new Complex[x_knot_count * y_knot_count];

            for (int j = 0; j < y_knot_count; j++)
            {
                for (int i = 0; i < x_knot_count; i++)
                {
                    sum_real += EM_array[i, j].Real;
                }
            }

            mean_real = sum_real / (x_knot_count * y_knot_count);

            int k = 0;
            for (int j = 0; j < y_knot_count; j++)
            {
                for (int i = 0; i < x_knot_count; i++)
                {
                    if (Math.Abs(EM_array[i, j].Real - mean_real) < eps)
                    {
                        output_array[k] = EM_array[i, j];
                        k++;
                    }
                }
            }

            Array.Resize(ref output_array, k);

            return output_array;
        }

        public override string ToString()
        {
            string output = $"  type = V2DataOnGrid\n";
            output += $"knot_count on OX axis = {grid_settings[0].knot_count}; knot_count on OY axis = {grid_settings[1].knot_count}\n";
            output += $"stride on OX axis = {grid_settings[0].stride}; stride on OY axis = {grid_settings[1].stride}\n";
            output += base.ToString();
            return output;
        }

        public override string ToLongString()
        {
            string output = this.ToString();
            for (int j = 0; j < grid_settings[1].knot_count; j++)
            {
                for (int i = 0; i < grid_settings[0].knot_count; i++)
                {
                    output += EM_array[i, j].ToString() + " ";
                }
                output += "\n";
            }
            return output;
        }

    }

    class V2DataCollection : V2Data
    {
        public List<DataItem> EM_list { get; set; }

        public V2DataCollection(string info, double EM_frequency) : base(info, EM_frequency)
        {
            EM_list = new List<DataItem>();
        }

        public void InitRandom(int nItems, float xmax, float ymax, double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                double tmp = maxValue;
                maxValue = minValue;
                minValue = tmp;
            }

            Random rnd = new Random();

            Vector2 coord;
            Complex num;
            DataItem data;
            for (int i = 0; i < nItems; i++)
            {
                coord = new Vector2((float)rnd.NextDouble() * xmax, (float)rnd.NextDouble() * ymax);
                num = new Complex(minValue + (maxValue - minValue) * rnd.NextDouble(), minValue + (maxValue - minValue) * rnd.NextDouble());
                data = new DataItem(coord, num);
                EM_list.Add(data);
            }

        }

        public override Complex[] NearAverage(float eps)
        {
            double mean_real = 0, sum_real = 0;
            int knot_count = EM_list.Count;
            Complex[] output_array = new Complex[knot_count];

            for (int i = 0; i < knot_count; i++)
            {
                sum_real += EM_list[i].EM_value.Real;
            }

            mean_real = sum_real / knot_count;

            int k = 0;
            for (int i = 0; i < knot_count; i++)
            {
                if (Math.Abs(EM_list[i].EM_value.Real - mean_real) < eps)
                {
                    output_array[k] = EM_list[i].EM_value;
                    k++;
                }
            }

            Array.Resize(ref output_array, k);

            return output_array;
        }

        public override string ToString()
        {
            string output = $"  type = V2DataCollection\n";
            output += $"knot_count = {EM_list.Count}\n";
            output += base.ToString();
            return output;
        }

        public override string ToLongString()
        {
            string output = this.ToString();
            for (int i = 0; i < EM_list.Count; i++)
            {
                output += "coord = " + EM_list[i].grid_coord.ToString() + "; value = " + EM_list[i].EM_value.ToString() + ";\n";
            }
            return output;
        }
    }

    class V2MainCollection : IEnumerable<V2Data>
    {
        private List<V2Data> V2data_list;
        public int Count { get { return V2data_list.Count; } }

        public V2MainCollection()
        {
            V2data_list = new List<V2Data>();
        }

        public void Add(V2Data item)
        {
            V2data_list.Add(item);
        }

        public void AddDefaults()
        {
            // random init 
            V2DataOnGrid data_grid = new V2DataOnGrid("data_grid_2", 1.0f, new Grid1D(1, 2), new Grid1D(2, 3));
            data_grid.InitRandom(-10.0, -5.0);

            V2DataCollection data_collection_1 = new V2DataCollection("data_collection_1", 2.0f);
            data_collection_1.InitRandom(3, 1.0f, 2.0f, 1.0f, 5.0f);

            V2DataCollection data_collection_2 = new V2DataCollection("data_collection_2", 3.0f);
            data_collection_2.InitRandom(5, 10.0f, 20.0f, -2.0f, 2.0f);

            V2data_list.Add(data_grid);
            V2data_list.Add(data_collection_1);
            V2data_list.Add(data_collection_2);
        }

        public bool Remove(string id, double w)
        {
            bool return_value = false;

            for (int i = 0; i < V2data_list.Count; i++)
            {
                if ((V2data_list[i].info == id) && (V2data_list[i].EM_frequency == w))
                {
                    V2data_list.Remove(V2data_list[i]);
                    return_value = true;
                    i--;
                }

            }
            return return_value;
        }

        public override string ToString()
        {
            string output = $"  type = V2MainCollection\n";
            for (int i = 0; i < V2data_list.Count; i++)
            {
                output += V2data_list[i].ToString() + "\n";
            }
            return output;
        }

        // interface implementation
        public IEnumerator GetEnumerator()
        {
            return V2data_list.GetEnumerator();
        }

        IEnumerator<V2Data> IEnumerable<V2Data>.GetEnumerator()
        {
            return V2data_list.GetEnumerator();
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            // ------------------------------

            Grid1D grid = new Grid1D(123.2334f, 67);
            Console.WriteLine(grid.ToString("F1"));

            

            // ------------------------------


        }
    }
}
