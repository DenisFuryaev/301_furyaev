using System;
using System.Collections.Generic;
using System.Numerics;

namespace FirstProject
{

    // grid item type
    struct DataItem
    {
        public Vector2 grid_coord { get; set; }
        Complex EM_value;

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
            // TODO: need to implement !!!
            Complex[] array = null;
            return array;
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

    //class V2DataCollection : V2DATA
    //{

    //}

    class Program
    {
        static void Main(string[] args)
        {
            Grid1D grid_x = new Grid1D(1, 2);
            Grid1D grid_y = new Grid1D(2, 3);
            //Console.WriteLine(grid_x);
            V2DataOnGrid data = new V2DataOnGrid("some information about this data", 10.0f, grid_x, grid_y);
            data.InitRandom(-10.0, 5.0);
            Console.WriteLine(data.ToLongString());

        }
    }
}
