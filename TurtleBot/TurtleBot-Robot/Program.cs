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
			this.javaPath = @"C:\Users\ryana\Documents\Test\Main.class";
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
            this.jarProcess.StartInfo.UseShellExecute = false;
            this.jarProcess.StartInfo.FileName = "C:\\\"Program Files\"\\Java\\jdk1.8.0_201\\bin\\java.exe";
            this.jarProcess.StartInfo.Arguments = this.javaPath;
            //this.jarProcess.StartInfo.Verb = "runas";
            //this.jarProcess.StartInfo.UseShellExecute = true;
            this.jarProcess.EnableRaisingEvents = true;
            this.jarProcess.Exited += this.DisposeJarProcess;
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
