using System;
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
    }

    abstract class V2DATA
    {
        public string info { get; set; }
        public double EM_frequency { get; set; }

        public V2DATA(string info, double EM_frequency)
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

    class V2DataOnGrid : V2DATA
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
                throw new System.InvalidOperationException("minValue must be less or equal than maxValue");
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

        // TODO: оператор преобразования к типу V2DataCollection !!!

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
            string output = $"knot_count on OX axis = {grid_settings[0].knot_count}; knot_count on OY axis = {grid_settings[1].knot_count}\n";
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

    class V2DataCollection : V2DATA
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
                throw new System.InvalidOperationException("minValue must be less or equal than maxValue");
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
            string output = $"knot_count = {EM_list.Count}\n";
            output += base.ToString();
            return output;
        }

        public override string ToLongString()
        {
            string output = this.ToString();
            for (int i = 0; i < EM_list.Count; i++)
            {
                output += "coord = " + EM_list[i].grid_coord.ToString() + "value = " + EM_list[i].EM_value.ToString() + " ";
            }
            return output;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Grid1D grid_x = new Grid1D(1, 2);
            Grid1D grid_y = new Grid1D(2, 3);
            V2DataOnGrid data = new V2DataOnGrid("some information about this data", 10.0f, grid_x, grid_y);
            data.InitRandom(-10.0, 5.0);

            Console.WriteLine(data.ToLongString());
            Complex[] array = data.NearAverage(2.0f);
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine(array[i]);
            }
            

        }
    }
}
