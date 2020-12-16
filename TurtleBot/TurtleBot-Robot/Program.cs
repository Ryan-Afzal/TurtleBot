using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TurtleBot_Robot {
	public class Program {

		public static async Task Main(string[] args) {
			DemoController demo = new DemoController();

			//string filename = "C:\\Users\\ryana\\Documents\\Test\\Main.java";
			string filename = Console.ReadLine();

			await demo.RunDemo(filename);
		}
	}
}
