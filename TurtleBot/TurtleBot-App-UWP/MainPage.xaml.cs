using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TurtleBot_App_UWP {
    public sealed partial class MainPage : Page {

        public static readonly string JAVAPATH = @"C:\Users\ryana\Documents\Test\";
        public static readonly string CODEPATH = @"Main.java";
        public static readonly string FILEPATH = @"Main.class";

        public MainPage() {
			this.InitializeComponent();
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e) {
            try {
                Debug.WriteLine("RunButton Clicked!");

                string input_path = JAVAPATH + CODEPATH;
                string output_path = JAVAPATH + FILEPATH;

                Debug.WriteLine("Compiling Code...");

                using (Process compiler = new Process()) {
                    compiler.StartInfo.UseShellExecute = false;
                    compiler.StartInfo.FileName = "C:\\\"Program Files\"\\Java\\jdk1.8.0_201\\bin\\javac";
                    compiler.StartInfo.CreateNoWindow = true;
                    compiler.StartInfo.Arguments = input_path;
                    compiler.Start();
                    compiler.WaitForExit();
                }

                Debug.WriteLine("Done Compiling!");

                //Debug.WriteLine("Running...");
                //Debug.WriteLine("Done!");
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

    }
}
