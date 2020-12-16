using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TurtleBot_Robot {
	/// <summary>
	/// Controller class for the demo.
	/// </summary>
	public class DemoController {

		public async Task RunDemo(string filename) {
			Console.WriteLine("Checking Input Path...");
			if (Path.IsPathFullyQualified(filename) && File.Exists(filename)) {
				Console.WriteLine("Path is valid.");

				string compileFile = filename;
				string runtimeFile = Path.ChangeExtension(filename, "class");

				Console.WriteLine("Compiling...");

				using Process compiler = new Process() {
					StartInfo = new ProcessStartInfo() {
						FileName = "javac",
						Arguments = compileFile,
						CreateNoWindow = false
					},
					EnableRaisingEvents = true
				};
				compiler.Disposed += (sender, e) => {
					Console.WriteLine("[Compiler Process Disposed]");
				};
				compiler.Start();
				await compiler.WaitForExitAsync();

				if (File.Exists(runtimeFile)) {
					Console.WriteLine("Done Compiling!");
					Console.WriteLine("Running...");

					using Process runtime = new Process() {
						StartInfo = new ProcessStartInfo() {
							FileName = "java",
							Arguments = Path.GetFileNameWithoutExtension(runtimeFile),
							WorkingDirectory = Path.GetDirectoryName(runtimeFile),
							//Verb = "runas",
							CreateNoWindow = false
						},
						EnableRaisingEvents = true
					};
					runtime.Disposed += (sender, e) => {
						Console.WriteLine("[Runtime Process Disposed]");
					};
					runtime.Start();
					await runtime.WaitForExitAsync();

					Console.WriteLine("Done!");
				} else {
					Console.Error.WriteLine("Compilation Error!");
				}
			} else {
				Console.Error.WriteLine("Invalid path!");
			}
		}

	}
}
