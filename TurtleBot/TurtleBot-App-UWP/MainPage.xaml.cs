using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TurtleBot_App_UWP {
    public sealed partial class MainPage : Page {

        private string sourcePath;
        private string javaPath;

        private Process javacProcess;
        private Process jarProcess;

        public MainPage() {
			this.InitializeComponent();

            this.sourcePath = null;
            this.javaPath = null;

            this.javacProcess = null;
            this.jarProcess = null;

            JarRunButton.IsEnabled = false;
            JarKillButton.IsEnabled = false;
        }

        private void DisposeJarProcess(object sender, System.EventArgs e) {
            Debug.WriteLine("Disposing JVM...");
            this.jarProcess.Dispose();
            this.jarProcess = null;

            JarRunButton.IsEnabled = true;
            JarKillButton.IsEnabled = false;
            Debug.WriteLine("Disposed JVM.");
		}

        /// <summary>
        /// Picks a <c>.class</c> or <c>.jar</c> file for the JVM to run.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private async void JarPickButton_Click(object sender, RoutedEventArgs e) {
			// Create a file picker and give it properties
            FileOpenPicker picker = new FileOpenPicker() {
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};

            // Add filters
			picker.FileTypeFilter.Add(".class");
            picker.FileTypeFilter.Add(".jar");
            picker.FileTypeFilter.Add(".java");

            // Pick File
            Debug.WriteLine("Picking file...");
            StorageFile pickedFile = await picker.PickSingleFileAsync();

            if (pickedFile is null) {
                // If the file isn't valid, real, or the user closed the dialog, clear the paths and disable the buttons
                this.javaPath = null;
                JarPickLabel.Text = "N/A";

                JarRunButton.IsEnabled = false;
                JarKillButton.IsEnabled = false;

                Debug.WriteLine($"Couldn't pick file: File was null");
            } else {
                // If the file is real, set the paths to it and enable the buttons
                this.javaPath = pickedFile.Path;
                JarPickLabel.Text = pickedFile.Path;

                JarRunButton.IsEnabled = true;
                JarKillButton.IsEnabled = false;

                Debug.WriteLine($"Picked file: {this.javaPath}");
            }
        }

		private void JarRunButton_Click(object sender, RoutedEventArgs e) {
            // Run the JVM with the picked file

            Debug.WriteLine("Starting JVM...");
            this.jarProcess = new Process();
            this.jarProcess.StartInfo.UseShellExecute = false;
            this.jarProcess.StartInfo.FileName = "C:\\\"Program Files\"\\Java\\jdk1.8.0_201\\bin\\java";
            this.jarProcess.StartInfo.Arguments = this.javaPath;
            this.jarProcess.EnableRaisingEvents = true;
            this.jarProcess.Exited += this.DisposeJarProcess;
            this.jarProcess.Start();
            Debug.WriteLine("JVM Started.");
        }

		private void JarKillButton_Click(object sender, RoutedEventArgs e) {
            // Kill the JVM process and dispose of it
            Debug.WriteLine("Killing JVM...");
            this.jarProcess.Kill();
            this.DisposeJarProcess(null, null);
            Debug.WriteLine("Killed JVM.");
        }

    }
}
