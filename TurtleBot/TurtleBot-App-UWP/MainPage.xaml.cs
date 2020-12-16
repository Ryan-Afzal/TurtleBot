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
using Windows.UI.Core;
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
        private string sourceFile;
        private string javaPath;
        private string javaFile;

        private Process javacProcess;
        private Process jarProcess;

        public MainPage() {
			this.InitializeComponent();

            this.sourcePath = null;
            this.javaPath = null;
            this.javaFile = null;

            this.javacProcess = null;
            this.jarProcess = null;

            SourceRunButton.IsEnabled = false;
            SourceKillButton.IsEnabled = false;

            JarRunButton.IsEnabled = false;
            JarKillButton.IsEnabled = false;
        }

        private void Output(object sender, DataReceivedEventArgs e) {
            ProcessOutput.Text += $"\n> {e.Data}";
		}

        /// <summary>
        /// Event to be called when JavaC exits, either by exiting normally or being killed by <see cref="SourceKillButton_Click(object, RoutedEventArgs)"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisposeJavacProcess(object sender, EventArgs e) {
            Debug.WriteLine("Disposing JavaC...");
            this.javacProcess.Dispose();
            this.javacProcess = null;

            SourceRunButton.IsEnabled = true;
            SourceKillButton.IsEnabled = false;
            Debug.WriteLine("Disposed JavaC.");
		}

        /// <summary>
        /// Event to be called when the JVM exits, either by exiting normally or being killed by <see cref="JarKillButton_Click(object, RoutedEventArgs)"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisposeJarProcess(object sender, EventArgs e) {
            Debug.WriteLine("Disposing JVM...");
            this.jarProcess.Dispose();
            this.jarProcess = null;

            JarRunButton.IsEnabled = true;
            JarKillButton.IsEnabled = false;
            Debug.WriteLine("Disposed JVM.");
        }

        /// <summary>
        /// Picks a .java file for JavaC to run.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SourcePickButton_Click(object sender, RoutedEventArgs e) {
            // Create a file picker and give it properties
            FileOpenPicker picker = new FileOpenPicker() {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            // Add filters
            picker.FileTypeFilter.Add(".java");

            // Pick File
            Debug.WriteLine("Picking file...");
            StorageFile pickedFile = await picker.PickSingleFileAsync();

            if (pickedFile is null) {
                // If the file isn't valid, real, or the user closed the dialog, clear the paths and disable the buttons
                this.sourcePath = null;
                SourcePickLabel.Text = "N/A";

                SourceRunButton.IsEnabled = false;
                SourceKillButton.IsEnabled = false;

                Debug.WriteLine($"Couldn't pick file");
            } else {
                // If the file is real, set the paths to it and enable the buttons
                SourcePickLabel.Text = pickedFile.Path;
                this.sourcePath = pickedFile.Path.Substring(0, pickedFile.Path.LastIndexOf('\\'));
                this.sourceFile = pickedFile.Name;
                //this.sourceFile = pickedFile.Name.Substring(0, pickedFile.Name.LastIndexOf('.'));

                SourceRunButton.IsEnabled = true;
                SourceKillButton.IsEnabled = false;

                Debug.WriteLine($"Picked file: {this.sourcePath}\\{this.sourceFile}");
            }
        }

        /// <summary>
        /// Runs JavaC.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceRunButton_Click(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Starting JavaC...");

            // Create the process
            this.javacProcess = new Process();
            this.javacProcess.StartInfo.FileName = "javac";

            // Set StartInfo properties, such as arguments
            this.javacProcess.StartInfo.Arguments = @"C:\Users\ryana\Documents\Test\Main.java";
            //this.javacProcess.StartInfo.Arguments = this.sourceFile;
            //this.javacProcess.StartInfo.WorkingDirectory = this.sourcePath;
            this.javacProcess.StartInfo.Verb = "runas";
            this.javacProcess.StartInfo.RedirectStandardOutput = true;
            this.javacProcess.StartInfo.RedirectStandardError = true;

            // Configure events
            this.javacProcess.EnableRaisingEvents = true;
			this.javacProcess.OutputDataReceived += async (object e_sender, DataReceivedEventArgs e_e) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    this.Output(e_sender, e_e);
                });
            };
            this.javacProcess.ErrorDataReceived += async (object e_sender, DataReceivedEventArgs e_e) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    this.Output(e_sender, e_e);
                });
            };
            this.javacProcess.Exited += async (object e_sender, EventArgs e_e) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    this.DisposeJavacProcess(e_sender, e_e);
                });
            };

            // Start the process
            this.javacProcess.Start();
            this.javacProcess.BeginOutputReadLine();
            this.javacProcess.BeginErrorReadLine();

            Debug.WriteLine("Started JavaC.");
        }

		/// <summary>
		/// Kills JavaC, which should also dispose of it and call <see cref="DisposeJavacProcess(object, EventArgs)"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SourceKillButton_Click(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Killing JavaC...");
            this.javacProcess.Kill();
            Debug.WriteLine("Killed JavaC.");
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

            // Pick File
            Debug.WriteLine("Picking file...");
            StorageFile pickedFile = await picker.PickSingleFileAsync();

            if (pickedFile is null) {
                // If the file isn't valid, real, or the user closed the dialog, clear the paths and disable the buttons
                this.javaPath = null;
                this.javaFile = null;
                JarPickLabel.Text = "N/A";

                JarRunButton.IsEnabled = false;
                JarKillButton.IsEnabled = false;

                Debug.WriteLine($"Couldn't pick file");
            } else {
                // If the file is real, set the paths to it and enable the buttons
                JarPickLabel.Text = pickedFile.Path;
                this.javaPath = pickedFile.Path.Substring(0, pickedFile.Path.LastIndexOf('\\'));
                this.javaFile = pickedFile.Name.Substring(0, pickedFile.Name.LastIndexOf('.'));

                JarRunButton.IsEnabled = true;
                JarKillButton.IsEnabled = false;

                Debug.WriteLine($"Picked file: {pickedFile.Path}");
            }
        }

        /// <summary>
        /// Runs the JVM.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void JarRunButton_Click(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Starting JVM...");

            this.jarProcess = new Process();
            this.jarProcess.StartInfo.FileName = "java";
            this.jarProcess.StartInfo.Arguments = this.javaFile;
            this.jarProcess.StartInfo.WorkingDirectory = this.javaPath;
            this.jarProcess.StartInfo.Verb = "runas";
            this.jarProcess.StartInfo.RedirectStandardOutput = true;
            this.jarProcess.StartInfo.RedirectStandardError = true;
            this.jarProcess.EnableRaisingEvents = true;
            this.jarProcess.OutputDataReceived += async (object e_sender, DataReceivedEventArgs e_e) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    this.Output(e_sender, e_e);
                });
            };
            this.jarProcess.ErrorDataReceived += async (object e_sender, DataReceivedEventArgs e_e) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    this.Output(e_sender, e_e);
                });
            };
            this.jarProcess.Exited += async (object e_sender, EventArgs e_e) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    this.DisposeJarProcess(e_sender, e_e);
                });
            };
            this.jarProcess.Start();
            this.jarProcess.BeginOutputReadLine();
            this.jarProcess.BeginErrorReadLine();

            Debug.WriteLine("JVM Started.");
        }

        /// <summary>
        /// Kills the JVM, which should also dispose of it and call <see cref="DisposeJarProcess(object, EventArgs)"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void JarKillButton_Click(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Killing JVM...");
            this.jarProcess.Kill();
            Debug.WriteLine("Killed JVM.");
        }
	}
}
