using piMovie.myClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
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

namespace piMovie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        String mPathName;


        // TCP 
        Int32 port = 9999;
        IPAddress host = IPAddress.Parse("192.168.0.9");


        public MainWindow()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                if(args[1] != null)
                {
                    connectToPi(args[1]);
                }
            }

            for(int i = 0; i < args.Length; i ++)
            {
                writeLog(args[i]);
            }
        }

        

        /*
         * This method will handle the sharing and connecting to 
         * the raspberryPie
         * In the future, move these actions to own classes.
         */
        private void connectToPi(String pathname)
        {

            try
            {

                if(pathname != "")
                {
                    mPathName = pathname;
                } else
                {
                    // Get the path name
                    mPathName = tbPathname.Text.ToString();
                }
 
                writeLog("Path to folder that will be used " + mPathName);

                // Init the management class
                ManagementClass mClass = new ManagementClass("Win32_share");

                // Create the in and outParams
                ManagementBaseObject inParams = mClass.GetMethodParameters("Create");
                ManagementBaseObject outParams;

                // Set inParams
                inParams["Description"] = "";
                inParams["Name"] = "e03";
                inParams["Path"] = mPathName;
                inParams["Type"] = 0x0;

                // Invoke the create method
                outParams = mClass.InvokeMethod("Create", inParams, null);


                if ((uint)(outParams.Properties["ReturnValue"].Value) != 0)
                {

                    throw new Exception("Unable to share directory.");
                }
                else
                {
                    writeLog("Folder shared.");
                }
            } catch (Exception ex)
            {
                writeLog(ex.ToString());
            }

            // Done  with sharing
            // Connect to server.
            TCPConnection conn = new TCPConnection();

            conn.ConnectToServer();

        }


        private void deleteShare()
        {
            // Get the scope of the searche
            ManagementScope ms = new ManagementScope(@"\\localhost\");

            // Cool, query in the scope 
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher(
                    "Select * from Win32_Share where Name ='e03'");

            // Result of the serach
            ManagementObjectCollection result = searcher.Get();

            // Delete each object
            // This will be handy in the future when we want
            // to delete more then one file
            foreach (ManagementObject obj in result)
                obj.InvokeMethod("Delete", new object[] { });
            writeLog("Share deleted");
        }


        // Handle the button click
        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            // If we want to delete the file
            if (tbPathname.Text == "del")
            {
                deleteShare();
            } else
            {
                connectToPi("");
            }      
        }

        // Log function
        private void writeLog(String message)
        {
            tBlockLog.Text += "\n[" + DateTime.Now.ToString("h:mm:ss tt") + "] - " + message;
        }
    }
}
