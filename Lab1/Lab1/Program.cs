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
            return $"coordinate - ({grid_coord.X}, {grid_coord.Y}); value = {EM_value}";
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
            return $"stride = {stride}; knot_count = {knot_count}";
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
            return $"info = {info}; EM_frequency = {EM_frequency}";
        }
    }

    class V2DataOnGrid : V2DATA
    {
        public Grid1D[] grid_settings { get; set; }
        Complex[,] EM_array;

        public V2DataOnGrid(string info, double EM_frequency, Grid1D OX_settings, Grid1D OY_settings) : base(info, EM_frequency)
        {
            EM_array = new Complex[OX_settings.knot_count, OY_settings.knot_count];
            grid_settings[0] = OX_settings;
            grid_settings[1] = OY_settings;
        }

        public void InitRandom(double minValue, double maxValue)
        {
            for (int j = 0; j < grid_settings[1].knot_count; j++)
            {
                for (int i = 0; i < grid_settings[0].knot_count; i++)
                {
                    // TODO: EM_valur need to be random !!!
                    EM_array[i, j] = new Complex(0.5, 0.5);
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

        public override string ToLongString()
        {
            // TODO: need to implement !!!
            return "wow";
        }

        public override string ToString()
        {
            return $"knot_count on OX axis = {grid_settings[0].knot_count}; knot_count on OY axis = {grid_settings[1].knot_count}";
        }
    }

    //class V2DataCollection : V2DATA
    //{

    //}

    class Program
    {
        static void Main(string[] args)
        {

            V2DataOnGrid data = new V2DataOnGrid("some info", 10.0f, new Grid1D(1, 5), new Grid1D(1, 8));
            Console.WriteLine(data);

        }
    }
}
