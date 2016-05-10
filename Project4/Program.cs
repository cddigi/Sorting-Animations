/*
 * Cornelius Donley 
 * Project 4
 * Program.cs
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
using System.Windows.Threading;


namespace Project4
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application app = new Application();
            app.Run(new SortingWindow());
        }
    }

    class SortingWindow : Window
    {
        //Globals  
        const int NUM_ALGOS = 2;
        const double WIDTH = 800.0, HEIGHT = 600.0;
        enum MenuItems { ArraySetUp, AnimationType };
        enum AnimationButtons { Start, Pause, Manual };
        enum SortingAlgorithms { InsertionSort, Quicksort };
        string[] sortingOptions = { "Insertion Sort", "Quicksort" };
        MenuItem[] menuItems = new MenuItem[2];
        Button[] animationButtons = new Button[3];
        GroupBox[] sortingGroupBoxes = new GroupBox[NUM_ALGOS];
        DispatcherTimer timer = new DispatcherTimer();
        InsertionSort insertionSort;
        Quicksort quicksort;

        public SortingWindow()
        {
            //Variables            
            var dockPanel = new DockPanel();
            var menuStackPanel = new StackPanel();
            var centerStackPanel = new StackPanel();
            var buttonStackPanel = new StackPanel();
            var mainMenu = new Menu();

            menuStackPanel.Orientation = Orientation.Horizontal;            
            buttonStackPanel.Orientation = Orientation.Horizontal;
            buttonStackPanel.VerticalAlignment = VerticalAlignment.Bottom;
            dockPanel.Children.Add(buttonStackPanel);
            dockPanel.Children.Add(menuStackPanel);
            dockPanel.Children.Add(centerStackPanel);
            DockPanel.SetDock(menuStackPanel, Dock.Top);
            DockPanel.SetDock(buttonStackPanel, Dock.Bottom);
            this.Width = WIDTH;
            this.Height = HEIGHT * 0.33;
            this.Content = dockPanel;
            this.Title = "Donley's Sorting Animations";

            //Main Menu
            for (int i = 0; i < menuItems.Length; i++)
                menuItems[i] = new MenuItem();  
            menuStackPanel.Children.Add(mainMenu);
            menuStackPanel.Background = mainMenu.Background;
            menuItems[(int)MenuItems.ArraySetUp].Header = "Array Set Up";
            menuItems[(int)MenuItems.AnimationType].Header = "Animation Type";
            menuItems[(int)MenuItems.ArraySetUp].Click += ArraySetupMenu_Click;
            foreach (MenuItem m in menuItems) 
            {
                m.Margin = new Thickness(5, 5, 10, 5);
                m.Padding = new Thickness(5);
                m.VerticalAlignment = VerticalAlignment.Center;
                m.Background = Brushes.LightGray;
                mainMenu.Items.Add(m);
            }

            //Sorting Types Submenu
            for (int i = 0; i < NUM_ALGOS; i++)
            {
                var sortingMenuItem = new MenuItem();
                sortingMenuItem.Header = sortingOptions[i];
                sortingMenuItem.Tag = (SortingAlgorithms)i;
                sortingMenuItem.IsCheckable = true;
                sortingMenuItem.StaysOpenOnClick = true;
                sortingMenuItem.Click += SortingMenuItem_Click;
                menuItems[(int)MenuItems.AnimationType].Items.Add(sortingMenuItem);
            }

            //Group Boxes
            for (int i = 0; i < NUM_ALGOS; i++)
            {
                sortingGroupBoxes[i] = new GroupBox();
                sortingGroupBoxes[i].Header = sortingOptions[i];
                sortingGroupBoxes[i].Margin = new Thickness(5, 10, 5, 10);
                sortingGroupBoxes[i].Height = 0;
                sortingGroupBoxes[i].Visibility = Visibility.Hidden;
                centerStackPanel.Children.Add(sortingGroupBoxes[i]);
            }

            //Buttons      
            for (int i = 0; i < animationButtons.Length; i++)
                animationButtons[i] = new Button();                
            animationButtons[(int)AnimationButtons.Start].Content = "Start/Continue Animation";
            animationButtons[(int)AnimationButtons.Pause].Content = "Pause Animtion";
            animationButtons[(int)AnimationButtons.Manual].Content = "Manual Step";
            for (int i = 0; i < animationButtons.Length; i++)
            {
                animationButtons[i].Tag = (AnimationButtons)i;
                animationButtons[i].Width = 200.0;
                animationButtons[i].Margin = new Thickness(20, 10, 20, 10);
                animationButtons[i].Padding = new Thickness(5);
                animationButtons[i].Click += AnimationButton_Click;                
                buttonStackPanel.Children.Add(animationButtons[i]);
            }

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            Reset();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var numChecked = NUM_ALGOS;
            var numDone = 0;
            foreach (MenuItem m in menuItems[(int)MenuItems.AnimationType].Items)
            {
                if ((SortingAlgorithms)m.Tag == SortingAlgorithms.InsertionSort && m.IsChecked)
                    if (!insertionSort.IsDone) insertionSort.Sort();
                    else numDone++;
                if ((SortingAlgorithms)m.Tag == SortingAlgorithms.InsertionSort && !m.IsChecked) numChecked--;
                if ((SortingAlgorithms)m.Tag == SortingAlgorithms.Quicksort && m.IsChecked)
                    if (!quicksort.IsDone) quicksort.Sort();
                    else numDone++;
                if ((SortingAlgorithms)m.Tag == SortingAlgorithms.Quicksort && !m.IsChecked) numChecked--;
            }
            if (numChecked == numDone) Reset();
        }

        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch((AnimationButtons)btn.Tag)
            {
                case AnimationButtons.Start:
                    menuItems[(int)MenuItems.AnimationType].IsEnabled = false;
                    animationButtons[(int)AnimationButtons.Start].IsEnabled = false;
                    animationButtons[(int)AnimationButtons.Pause].IsEnabled = true;
                    animationButtons[(int)AnimationButtons.Manual].IsEnabled = false;
                    timer.Start();
                    break;
                case AnimationButtons.Manual:
                    menuItems[(int)MenuItems.AnimationType].IsEnabled = false;
                    Timer_Tick(sender, e);
                    break;
                case AnimationButtons.Pause:
                    animationButtons[(int)AnimationButtons.Start].IsEnabled = true;
                    animationButtons[(int)AnimationButtons.Pause].IsEnabled = false;
                    animationButtons[(int)AnimationButtons.Manual].IsEnabled = true;
                    timer.Stop();
                    break;
            }
        }

        private void ArraySetupMenu_Click(object sender, RoutedEventArgs e)
        {
            var size = DialogBox("New Random Array", "Enter size of the array (2-50):");
            if (size < 2 || size > 50) return;
            var arr = PopulateArray(size);
            insertionSort = new InsertionSort(arr);
            insertionSort.Height = HEIGHT * 0.33;
            insertionSort.Width = WIDTH * 0.95;
            insertionSort.DrawBarGraph();
            sortingGroupBoxes[(int)SortingAlgorithms.InsertionSort].Content = insertionSort.Canvas;
            quicksort = new Quicksort(arr);
            quicksort.Height = HEIGHT * 0.33;
            quicksort.Width = WIDTH * 0.95;
            quicksort.DrawBarGraph();    
            sortingGroupBoxes[(int)SortingAlgorithms.Quicksort].Content = quicksort.Canvas;
            menuItems[(int)MenuItems.AnimationType].IsEnabled = true;
            var m = sender as MenuItem;
            m.IsEnabled = false;
            bool b = false;
            foreach (MenuItem mm in menuItems[(int)MenuItems.AnimationType].Items)
                if (mm.IsChecked) b = true;
            animationButtons[(int)AnimationButtons.Start].IsEnabled = b;
            animationButtons[(int)AnimationButtons.Manual].IsEnabled = b;
        }

        private void SortingMenuItem_Click(object sender, RoutedEventArgs e)
        {            
            var m = sender as MenuItem;
            bool b = false;
            foreach (MenuItem mm in menuItems[(int)MenuItems.AnimationType].Items)
                if (mm.IsChecked) b = true;
            animationButtons[(int)AnimationButtons.Start].IsEnabled = b;
            animationButtons[(int)AnimationButtons.Manual].IsEnabled = b;
            switch ((SortingAlgorithms)m.Tag)
            {
                case SortingAlgorithms.InsertionSort:
                    if (m.IsChecked)
                    {
                        sortingGroupBoxes[(int)SortingAlgorithms.InsertionSort].Visibility = Visibility.Visible;
                        sortingGroupBoxes[(int)SortingAlgorithms.InsertionSort].Height = HEIGHT * 0.37;
                        this.Height += HEIGHT * 0.34;
                    }
                    else
                    {
                        sortingGroupBoxes[(int)SortingAlgorithms.InsertionSort].Visibility = Visibility.Hidden;
                        sortingGroupBoxes[(int)SortingAlgorithms.InsertionSort].Height = 0;
                        this.Height -= HEIGHT * 0.34;
                    }
                    break;
                case SortingAlgorithms.Quicksort:
                    if (m.IsChecked)
                    {
                        sortingGroupBoxes[(int)SortingAlgorithms.Quicksort].Visibility = Visibility.Visible;
                        sortingGroupBoxes[(int)SortingAlgorithms.Quicksort].Height = HEIGHT * 0.37;
                        this.Height += HEIGHT * 0.34;
                    }
                    else
                    {
                        sortingGroupBoxes[(int)SortingAlgorithms.Quicksort].Visibility = Visibility.Hidden;
                        sortingGroupBoxes[(int)SortingAlgorithms.Quicksort].Height = 0;
                        this.Height -= HEIGHT * 0.34;
                    }
                    break;
            }
        }

        void Reset()
        {
            menuItems[(int)MenuItems.ArraySetUp].IsEnabled = true;
            menuItems[(int)MenuItems.AnimationType].IsEnabled = false;            
            animationButtons[(int)AnimationButtons.Start].IsEnabled = false;
            animationButtons[(int)AnimationButtons.Pause].IsEnabled = false;
            animationButtons[(int)AnimationButtons.Manual].IsEnabled = false;
            timer.Stop();
        }

        double [] PopulateArray(int size)
        {
            var rand = new Random();
            var arr = new double[size];
            for (int i = 0; i < size; i++)
                arr[i] = rand.NextDouble();
            return arr;
        }

        //Creates dialog box with default value of 10
        int DialogBox(string title, string text, string defaultText = "10")
        {
            var size = Microsoft.VisualBasic.Interaction.InputBox(text, title, defaultText);
            int n;
            if (!int.TryParse(size, out n)) n = 0;
            return n;
        }
        
    }
}
