/*
 * Cornelius Donley 
 * Project 4
 * SortingAnimation.cs
 * This program animates the Insertion Sort and Quicksort algorithms
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Project4
{
    abstract class SortingAnimation
    {
        protected double[] array;
        protected Label[] colorBars;
        Random rand;

        public Canvas Canvas { get; private set; }    
        public bool IsDone { get; protected set; }    
        public double Height { private get { return this.Canvas.Height; } set { this.Canvas.Height = value; } }   
        public double Width { private get { return this.Canvas.Width; } set { this.Canvas.Width = value; } }

        public SortingAnimation(double[] arr, int seed = 13)
        {
            this.Canvas = new Canvas();
            rand = new Random(seed);
            array = arr;
            colorBars = new Label[array.Length];
            for (int i = 0; i < colorBars.Length; i++)
            {
                colorBars[i] = new Label();
                colorBars[i].BorderThickness = new Thickness(2);
                colorBars[i].BorderBrush = Brushes.Black;
                colorBars[i].Tag = i;

                var R = rand.Next(256);
                var G = rand.Next(256);
                var B = rand.Next(256);
                colorBars[i].Background = new SolidColorBrush(Color.FromRgb((byte)R, (byte)G, (byte)B));

                Canvas.SetBottom(colorBars[i], 0);
                this.Canvas.Children.Add(colorBars[i]);
            }
        }

        public void DrawBarGraph()
        {
            for (int i = 0; i < colorBars.Length; i++)
            {
                colorBars[i].Height = this.Canvas.Height * array[(int)colorBars[i].Tag];
                colorBars[i].Width = this.Canvas.Width / colorBars.Length;
                Canvas.SetLeft(colorBars[i], this.Canvas.Width / colorBars.Length * i);
            }
        }

        public abstract void Sort();
    }

    class InsertionSort : SortingAnimation
    {
        int i = 1, k = 1;

        public InsertionSort(double[] arr, int seed = 13) : base(arr, seed)
        {

        }

        public override void Sort()
        {
            if(k == colorBars.Length)
            {
                IsDone = true;
                return;
            }
            if(k == 0 || array[(int)colorBars[k].Tag] >= array[(int)colorBars[k - 1].Tag])
            {
                i++;
                k = i;
                return;
            }
            if (array[(int)colorBars[k].Tag] < array[(int)colorBars[k - 1].Tag])
            {
                object tempTag = colorBars[k].Tag;
                Brush tempBrush = colorBars[k].Background;
                colorBars[k].Tag = colorBars[k - 1].Tag;
                colorBars[k].Background = colorBars[k - 1].Background;
                colorBars[k - 1].Tag = tempTag;
                colorBars[k - 1].Background = tempBrush;
                colorBars[k].Height = this.Canvas.Height * array[(int)colorBars[k].Tag];
                colorBars[k - 1].Height = this.Canvas.Height * array[(int)colorBars[k - 1].Tag];
                k--;
            }
        }
    }

    class Quicksort : SortingAnimation
    {
        Stack<Range> stack;
        Range range;
        bool done;
        int lower, upper;

        class Range
        {
            public Range(int lower, int pivot)
            {
                Lower = lower;
                Pivot = pivot;
            }
            public int Lower { get; set; }
            public int Pivot { get; set; }
            public double Value { get; set; }
        }

        public Quicksort(double[] arr, int seed = 13) : base(arr, seed)
        {
            stack = new Stack<Range>();
            range = new Range(0, array.Length - 1);
            lower = range.Lower;
            upper = range.Pivot - 1;
            range.Value = array[(int)colorBars[range.Pivot].Tag];
            done = false;
        }

        public override void Sort()
        {
            if (!done) Partition();
            else if (stack.Count > 0)
            {
                range = stack.Pop();
                lower = range.Lower;
                upper = range.Pivot - 1;
                range.Value = array[(int)colorBars[range.Pivot].Tag];
                Partition();
            }
            
        }

        private void Partition()
        {
            //if (lower > range.Pivot) return;
            while (lower < range.Pivot && array[(int)colorBars[lower].Tag] < array[(int)colorBars[range.Pivot].Tag])
            {
                lower++;
            }
            if (lower < upper)
            {
                Swap(lower, upper);
                upper--;
                done = false;
            }
            else
            {
                Swap(lower, range.Pivot);
                done = true;
                if (lower + 1 < range.Pivot)
                {
                    stack.Push(new Range(lower + 1, range.Pivot));
                }
                if (lower - 1 > range.Lower)
                {
                    stack.Push(new Range(range.Lower, lower - 1));
                }
                if (stack.Count == 0) IsDone = true;
            }
        }

        private void Swap(int a, int b)
        {
            var t = colorBars[a].Tag;
            var bg = colorBars[a].Background;
            colorBars[a].Tag = colorBars[b].Tag;
            colorBars[a].Background = colorBars[b].Background;
            colorBars[a].Height = this.Canvas.Height * array[(int)colorBars[a].Tag];
            colorBars[b].Tag = t;
            colorBars[b].Background = bg;
            colorBars[b].Height = this.Canvas.Height * array[(int)colorBars[b].Tag];
        }
    }
}
