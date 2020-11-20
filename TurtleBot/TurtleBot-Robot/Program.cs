using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TurtleBot_Robot {
	public class Program {
		public static async Task Main(string[] args) {
            Program p = new Program();
            p.JarRunButton_Click();
		}

		private string javaPath;
		private Process jarProcess;

		public Program() {
            this.javaPath = @"Main";
			this.jarProcess = null;
		}

        private void DisposeJarProcess(object sender, EventArgs e) {
            Debug.WriteLine("Disposing JVM...");
            Console.WriteLine("Disposing JVM...");
            this.jarProcess.Dispose();
            this.jarProcess = null;
            Debug.WriteLine("Disposed JVM.");
            Console.WriteLine("Disposed JVM.");
        }

        private void JarRunButton_Click() {
            // Run the JVM with the picked file

            Debug.WriteLine("Starting JVM...");
            Console.WriteLine("Starting JVM...");
            this.jarProcess = new Process();
            this.jarProcess.StartInfo.FileName = "java";
            this.jarProcess.StartInfo.Arguments = this.javaPath;
            this.jarProcess.StartInfo.Verb = "runas";
            this.jarProcess.StartInfo.WorkingDirectory = @"C:\Users\ryana\Documents\Test";
            this.jarProcess.EnableRaisingEvents = true;
            this.jarProcess.Exited += new EventHandler(this.DisposeJarProcess);
            this.jarProcess.Start();
            Debug.WriteLine("JVM Started.");
            Console.WriteLine("JVM Started.");
        }

        private void JarKillButton_Click() {
            // Kill the JVM process and dispose of it
            Debug.WriteLine("Killing JVM...");
            Console.WriteLine("Killing JVM...");
            this.jarProcess.Kill();
            this.DisposeJarProcess(null, null);
            Debug.WriteLine("Killed JVM.");
            Console.WriteLine("Killed JVM.");
        }
    }
}
