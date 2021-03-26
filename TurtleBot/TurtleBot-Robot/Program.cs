using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TurtleBot_Robot {
	public class Program {

		public static async Task Main(string[] args) {
			IDemoController demo = new BluetoothServerDemoController();

			await demo.RunDemo();
		}
	}
}
