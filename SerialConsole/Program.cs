using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;
using Zh.ArduinoSerial;

namespace SerialConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            var instance = new Program();
            instance.Run().Wait();

            Console.WriteLine("Press Enter to Terminate Program");
            Console.ReadLine();
        }

        private async Task Run()
        {
            //ListAvailiblePorts();

            var spf = new SerialPortFactory();
            var serialPort = spf.Create("COM3", baudRate: 115200);

            void HandleDataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                int x = 0;
                var data = serialPort.ReadLine();
                Console.WriteLine(data);
            }

            serialPort.DataReceived += HandleDataReceived;
            serialPort.Open();

            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalMinutes < 1 ||
                   serialPort.IsOpen)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(5));
            }
        }

        private static void ListAvailiblePorts()
        {
            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }
        }
    }
}
