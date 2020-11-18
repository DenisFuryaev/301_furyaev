using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Linq;
using System.Globalization;
using System.ComponentModel.Design;

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
            output += "knot_count = " + knot_count.ToString() + ";\n";
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
        public abstract string ToLongString(string format);
        public abstract double GetAverage();
        public abstract IEnumerable<Vector2> GetCoords();

        public override string ToString()
        {
            return $"info = {info}; EM_frequency = {EM_frequency}\n";
        }

        public string ToString(string format)
        {
            string output = "info = " + info + "; ";
            output += "EM_frequency = " + EM_frequency.ToString(format) + ";\n";
            return output;
        }
    }

    class V2DataOnGrid : V2Data, IEnumerable<DataItem>
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

        /*
         (data format in file named filename)

         descriptions   - (string)
         EM_frequency   - (float with point, not comma)
         stride_OX, knot_count_OX   - (float, int)
         stride_OY, knot_count_OY   - (float, int)
         (x_1,1, y_1,1) ... (x_1,knot_count_OX, y_1,knot_count_OX)  - (float, float)
           ...                              ...
           ...                                               ...
           ...                                                              ...
         (x_knot_count_OY,1, y_knot_count_OY,1) ... (x_knot_count_OY,knot_count_OX, y_knot_count_OY,knot_count_OX)  - (float, float)

         Example:

         "date - 16/11/2020"
         12
         1 2
         3 2
         (-1.1, 1.2) (-1.3, 0.9)
         (-4.1, 1.6) (3.4, -4.2)
        */

        public V2DataOnGrid(string filename) : base("", 0)
        {
            string path = "../../../" + filename;

            try 
            {
                string[] lines = System.IO.File.ReadAllLines(path);

                string info = "";
                double EM_frequency;
                Grid1D OX_settings;
                Grid1D OY_settings;

                char[] delimiterChars = { ' ', ',', '(', ')' };

                info = lines[0];
                EM_frequency = Convert.ToDouble(lines[1]);

                string[] numbers = lines[2].Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                OX_settings = new Grid1D((float)Convert.ToDouble(numbers[0]), Convert.ToInt32(numbers[1]));
                Array.Clear(numbers, 0, numbers.Length);

                numbers = lines[3].Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                OY_settings = new Grid1D((float)Convert.ToDouble(numbers[0]), Convert.ToInt32(numbers[1]));
                Array.Clear(numbers, 0, numbers.Length);


                grid_settings = new Grid1D[2];
                grid_settings[0] = OX_settings;
                grid_settings[1] = OY_settings;

                EM_array = new Complex[OX_settings.knot_count, OY_settings.knot_count];
                int x = 0, y = 0;

                for (int i = 4; i < lines.Length; i++)
                {
                    numbers = lines[i].Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

                    for (x = 0; x < OX_settings.knot_count; x++)
                        EM_array[x, y] = new Complex(double.Parse(numbers[2 * x], System.Globalization.CultureInfo.InvariantCulture), double.Parse(numbers[2 * x + 1], System.Globalization.CultureInfo.InvariantCulture));

                    y++;
                    Array.Clear(numbers, 0, numbers.Length);
                }

                base.info = info;
                base.EM_frequency = EM_frequency;
            }
            catch(Exception e)
            {
                throw e; 
            }
            finally
            {
                Console.WriteLine("  The 'try catch' block is finished.");
            }

        }

        public override double GetAverage()
        {
            double average = 0, sum = 0;
            for (int j = 0; j < grid_settings[1].knot_count; j++)
            {
                for (int i = 0; i < grid_settings[0].knot_count; i++)
                {
                    sum += EM_array[i, j].Magnitude;
                }
            }
            average = sum / grid_settings[0].knot_count * grid_settings[1].knot_count;
            return average;
        }

        public override IEnumerable<Vector2> GetCoords()
        {
            yield return new Vector2(0f, 0f);
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
            string output = $"  type = " + this.GetType() + "\n";
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
            output += "\n";

            return output;
        }

        public override string ToLongString(string format)
        {
            string output = $"  type = " + this.GetType() + "\n";
            output += base.ToString(format);
            output += "grid_settings on OX axis: " + grid_settings[0].ToString(format);
            output += "grid_settings on OY axis: " + grid_settings[1].ToString(format);

            for (int j = 0; j < grid_settings[1].knot_count; j++)
            {
                for (int i = 0; i < grid_settings[0].knot_count; i++)
                {
                    output += EM_array[i, j].ToString(format) + " ";
                }
                output += "\n";
            }
            output += "\n";

            return output;
        }

        // interface implementation
        public IEnumerator GetEnumerator()
        {
            Vector2 coord = new Vector2(0.0f, 0.0f);
            float x_stride = grid_settings[0].stride, y_stride = grid_settings[1].stride;
            DataItem data_item;
            for (int j = 0; j < grid_settings[1].knot_count; j++)
            {
                for (int i = 0; i < grid_settings[0].knot_count; i++)
                {
                    data_item = new DataItem(coord, EM_array[i, j]);
                    yield return data_item;
                    coord.X += x_stride;
                }
                coord.Y += y_stride;
                coord.X = 0;
            }
        }

        IEnumerator<DataItem> IEnumerable<DataItem>.GetEnumerator()
        {
            Vector2 coord = new Vector2(0.0f, 0.0f);
            float x_stride = grid_settings[0].stride, y_stride = grid_settings[1].stride;
            DataItem data_item;
            for (int j = 0; j < grid_settings[1].knot_count; j++)
            {
                for (int i = 0; i < grid_settings[0].knot_count; i++)
                {
                    data_item = new DataItem(coord, EM_array[i, j]);
                    yield return data_item;
                    coord.X += x_stride;
                }
                coord.Y += y_stride;
                coord.X = 0;
            }
        }

    }

    class V2DataCollection : V2Data, IEnumerable<DataItem>
    {
        public List<DataItem> EM_list { get; set; }

        public override IEnumerable<Vector2> GetCoords()
        {
            foreach (DataItem item in EM_list)
                yield return item.grid_coord;
        }

        public V2DataCollection(string info, double EM_frequency) : base(info, EM_frequency)
        {
            EM_list = new List<DataItem>();
        }

        public override double GetAverage()
        {
            return EM_list.Average<DataItem>(x => x.EM_value.Magnitude);
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
            string output = $"  type = " + this.GetType() + "\n";
            output += $"knot_count = {EM_list.Count}\n";
            output += base.ToString();
            return output;
        }

        public override string ToLongString()
        {
            string output = this.ToString();

            for (int i = 0; i < EM_list.Count; i++)
                output += EM_list[i].ToString();
            output += "\n";

            return output;
        }

        public override string ToLongString(string format)
        {
            string output = $"  type = " + this.GetType() + "\n";
            output += base.ToString(format);

            for (int i = 0; i < EM_list.Count; i++)
                output += EM_list[i].ToString(format);
            output += "\n";

            return output;
        }

        // interface implementation
        public IEnumerator GetEnumerator()
        {
            return EM_list.GetEnumerator();
        }

        IEnumerator<DataItem> IEnumerable<DataItem>.GetEnumerator()
        {
            return EM_list.GetEnumerator();
        }
    }

    class V2MainCollection : IEnumerable<V2Data>
    {
        private List<V2Data> V2data_list;
        public int GetCount { get { return V2data_list.Count; } }
        public double GetAverage { get { return V2data_list.Average<V2Data>(x => x.GetAverage()); } }
        public double GetNearAverage
        {
            get
            {
                Console.WriteLine("  Value closest to the average value of all elements");
                V2Data item = V2data_list.OrderBy(x => x.EM_frequency).First();
                return item.EM_frequency;
            } 
        }
        public IEnumerable<Vector2> GetValue 
        {
            get
            {
                Console.WriteLine("  Coordinates of all elements in V2DataCollection members only");
                IEnumerable<IEnumerable<Vector2>> query =
                    from data in V2data_list
                    where data.GetType().Equals(typeof(V2DataCollection))
                    select data.GetCoords();

                return query.SelectMany(x => x);
            }
        }

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
            V2DataOnGrid data_grid = new V2DataOnGrid("data_grid_2", 6.0f, new Grid1D(1, 2), new Grid1D(2, 3));
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
            string output = $"  type = " + this.GetType() + "\n";

            for (int i = 0; i < V2data_list.Count; i++)      
                output += V2data_list[i].ToString() + "\n";
            
            return output;
        }

        public string ToLongString(string format = "G")
        {
            string output = "";

            for (int i = 0; i < V2data_list.Count; i++)
                output += V2data_list[i].ToLongString(format);

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

            try
            {
                V2DataOnGrid data_on_grid = new V2DataOnGrid("V2DataOnGrid.txt");
                Console.WriteLine(data_on_grid.ToLongString("N"));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return;
            }

            V2MainCollection main_collection = new V2MainCollection();
            main_collection.AddDefaults();
            Console.WriteLine(main_collection.ToLongString());

            foreach (Vector2 coord in main_collection.GetValue)
            {
                Console.WriteLine(coord);
            }

            Console.WriteLine(main_collection.GetNearAverage);

            // ------------------------------


        }
    }
}
