using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace metexsrv
{
    class Program
    {
        class Configuration
        {
            // Defaul Server Port
            public static Int16 Port = 12345;

            // Default COM-Port name
            public static String COMPort = "COM1";

            // Testmodus flag
            public static Boolean TestMode = false;
        }

        // Globale geraet Instanz
        public static Device device = null;

        static void Main(string[] args)
        {
            // Kommandozeilen parameter parsen
            ParseArguments(args);

            // Virtuelles geraet erstellen
            if (Program.Configuration.TestMode)
                device = new TestDevice();
            else
                device = new Metex(Program.Configuration.COMPort);

            // Port oeffnen
            device.Open();

            // Server starten
            Server server = new Server();
            server.Listen(Program.Configuration.Port);

            // 'index.html' im default browser oeffnen
            OpenAppPage();

            // Auf Tastatureingabe warten
            Console.ReadKey();
        }

        static void ParseArguments(String[] args)
        {
            for (Int32 argIndex = 0; argIndex < args.Length; ++argIndex)
            {
                switch (args[argIndex])
                {
                    case "-t":
                        Program.Configuration.TestMode = true;
                        break;
                    case "-i":
                        if (argIndex < args.Length)
                            Program.Configuration.COMPort = args[argIndex++];
                        else
                            Console.WriteLine("Missing COM port name");
                        break;
                    case "-p":
                        if (argIndex < args.Length)
                            Program.Configuration.Port = Convert.ToInt16(args[argIndex++]);
                        else
                            Console.WriteLine("Missing port number");
                        break;
                    default:
                        Console.WriteLine("Illegal argument");
                        break;
                }
            }
        }

        static void OpenAppPage()
        {
            const String fileName = "index.html";

            if (File.Exists(fileName))
            {
                Process.Start(fileName);
            }
            else
            if (File.Exists("..\\web\\" + fileName))
            {
                Process.Start("..\\web\\" + fileName);
            }
        }
    }
}
