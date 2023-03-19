using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            IntPtr lastWindowHandle = GetWindow(Process.GetCurrentProcess().MainWindowHandle, (uint)GetWindow_Cmd.GW_HWNDNEXT);
            
            while (true)
            {
                IntPtr temp = GetParent(lastWindowHandle);
                if (temp.Equals(IntPtr.Zero)) break;
                lastWindowHandle = temp;
            }
            SetForegroundWindow(lastWindowHandle);
            
            var inputSimulator = new InputSimulator();
            for (int i = 0; i < 2; i++)
            {
                inputSimulator.Keyboard.KeyDown(VirtualKeyCode.TAB);
                inputSimulator.Keyboard.KeyUp(VirtualKeyCode.TAB);
            }
            
            inputSimulator.Keyboard.KeyDown(VirtualKeyCode.SPACE);
            inputSimulator.Keyboard.KeyUp(VirtualKeyCode.SPACE);

            // for (int i = 0; i < 7; i++)  
            // {
            //     inputSimulator.Keyboard.KeyDown(VirtualKeyCode.TAB);
            //     inputSimulator.Keyboard.KeyUp(VirtualKeyCode.TAB);
            // }
            //
            // inputSimulator.Keyboard.KeyDown(VirtualKeyCode.SPACE);
            // inputSimulator.Keyboard.KeyUp(VirtualKeyCode.SPACE);

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