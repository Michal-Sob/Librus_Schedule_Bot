using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using LibrusScheduleHelper.Helpers;
using WindowsInput;
using WindowsInput.Native;

namespace LibrusScheduleHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand MyCommand = new RoutedCommand();
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }
        
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        
        public MainWindow()
        {
            InitializeComponent();
            MyCommand.InputGestures.Add(new KeyGesture(Key.Tab));

        }

        private void StartBot_Click(object sender, RoutedEventArgs e)
        {
            IntPtr lastWindowHandle =
                GetWindow(Process.GetCurrentProcess().MainWindowHandle, (uint)GetWindow_Cmd.GW_HWNDNEXT);

            while (true)
            {
                IntPtr temp = GetParent(lastWindowHandle);
                if (temp.Equals(IntPtr.Zero)) break;
                lastWindowHandle = temp;
            }

            SetForegroundWindow(lastWindowHandle);

            // Loading from a file, you can also load from a stream
            var xml = XDocument.Load(@"D:\OC\projekty\LibrusNetCore\LibrusScheduleHelper\Core\curriculums.xml");
            var reader = new XmlTextReader(@"D:\OC\projekty\LibrusNetCore\LibrusScheduleHelper\Core\curriculums.xml");

            while (reader.Read())
                Console.WriteLine(reader.Name);

            // Query the data and write out a subset of contacts
            /*var query = xml.Root.Descendants("Curriculum")
                .Where(c => (int)c.Attribute("id") == 20180130)
                .Select(c => c.Element("Level").Elements())
                .First()
                .ToArray();*/

            var query = xml.Root.Descendants("Curriculum")
                .Where(c => (int)c.Attribute("id") == 20170214)
                .Descendants("Stage")
                .Where(c => (int)c.Attribute("id") == 2)
                .Descendants("Subject")
                .Where(c => (string)c.Attribute("id") == "jezykObcyNowozytny")
                .Descendants("Level")
                .Where(c => (string)c.Attribute("id") == "II.1")
                .Select(c => c.Elements())
                .First();

            var curriculum = query.Select(x => int.Parse(x.Value)).ToArray();

            var lines = File.ReadLines(@"D:\OC\projekty\LibrusNetCore\LibrusScheduleHelper\iSucceed_A1_RM_II.1_90_CSV.csv").ToArray();
            var searchItem = new Regex(@"\b[IVXLCDM]+\.\d+\b");
            var romanToNumbers = new RomanToNumbers();
            var result = new List<(string,int,int)>();
            
            foreach (string line in lines)
            {
                if (searchItem.IsMatch(line))
                {
                    var points = line.Split(';');
                    points = points.Skip(7).ToArray();
                    var p = String.Join(';', points);
                    var g = p.Split(',', ';');

                    foreach (var point in g)
                    {
                        var rn = romanToNumbers.RomanToInt(point);

                        var f = Regex.Match(point, @"\d+").Value;
                        var resultString = 0;
                        
                        if (f.Any()) 
                            resultString = int.Parse(f);

                        result.Add((point, rn, resultString == 0 ? 1 : resultString));
                    }
                    result.Add(("END", 0, 0));
                }
            }
            
            var inputSimulator = new InputSimulator();
            for (var pointIdx = 0; pointIdx < result.Count;)
            {
                var point = result[pointIdx];
                
                if (point.Item2 == 0)
                    WaitForNextPoint();
                
                
                for (var i = 1; i < curriculum.Length; i++)
                {
                    if (point.Item2 == 0)
                        break;

                    if (curriculum[i] > 1)
                    {
                        //ClickButton(inputSimulator, VirtualKeyCode.TAB, 200);
                    }
      

                    for (var j = 1; j < curriculum[i] + 1; j++)
                    {
                        //ClickButton(inputSimulator, VirtualKeyCode.TAB, 300);

                        if (i == point.Item2 && j == point.Item3)
                        {
                            //ClickButton(inputSimulator, VirtualKeyCode.SPACE, 300);

                            pointIdx++;
                            point = result[pointIdx];

                            if (point.Item2 == 0)
                                break;
                        }       
                    }
                }
            }

        }

        private void ClickButton(InputSimulator inputSimulator, VirtualKeyCode button, int waitMs)
        {
            inputSimulator.Keyboard.KeyDown(button);
            inputSimulator.Keyboard.KeyUp(button);
            Thread.Sleep(waitMs);
        }

        private void WaitForNextPoint()
        {
            while (Console.Read() == -1) {
            }
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //var key = Key.A;                    // Key to send
            //var target = Keyboard.FocusedElement;    // Target element
            //var routedEvent = Keyboard.KeyDownEvent; // Event to send
            //MessageBox.Show(Keyboard.PrimaryDevice.ActiveSource.ToString());

            var inputSimulator = new InputSimulator();
            for (int i = 0; i < 50; i++)
            {
                inputSimulator.Keyboard.KeyDown(VirtualKeyCode.NUMPAD1);
                inputSimulator.Keyboard.KeyUp(VirtualKeyCode.NUMPAD1);
            }
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("DZIAŁA");
        }
    }
}