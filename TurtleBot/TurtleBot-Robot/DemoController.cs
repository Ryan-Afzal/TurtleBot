using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace TurtleBot_Robot {
	public class DemoController {

		public async Task RunDemo(string filename) {
			bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
			
			Console.WriteLine("Checking Input Path...");
			if (Path.IsPathFullyQualified(filename) && File.Exists(filename)) {
				Console.WriteLine("Path is valid.");

				string compileFile = filename;
				string runtimeFile = Path.ChangeExtension(filename, "class");

				if (File.Exists(runtimeFile)) {
					File.Delete(runtimeFile);
				}

				Console.WriteLine("Compiling...");

				using (Process compiler = new Process()) {
					compiler.StartInfo = new ProcessStartInfo() {
						FileName = "javac",
						Arguments = compileFile,
						CreateNoWindow = false
					};
					compiler.EnableRaisingEvents = true;
					compiler.Disposed += (sender, e) => {
						Console.WriteLine("[Compiler Process Disposed]");
					};
					compiler.Start();
					await compiler.WaitForExitAsync();
				}

				if (File.Exists(runtimeFile)) {
					Console.WriteLine("Done Compiling!");
					Console.WriteLine("Running...");

					using (Process runtime = new Process()) {
						runtime.StartInfo = new ProcessStartInfo() {
							FileName = "java",
							Arguments = Path.GetFileNameWithoutExtension(runtimeFile),
							WorkingDirectory = Path.GetDirectoryName(runtimeFile),
							CreateNoWindow = false
						};
						runtime.EnableRaisingEvents = true;
						runtime.Disposed += (sender, e) => {
							Console.WriteLine("[Runtime Process Disposed]");
						};
						runtime.Start();
						await runtime.WaitForExitAsync();
					}
					

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
