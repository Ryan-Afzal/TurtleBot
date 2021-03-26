using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Bluetooth_UWP_Client {
	public sealed class DeviceInformationViewModel {
		public string Id { get; set; }
		public DeviceInformation DeviceInformation { get; set; }
		public string IsPaired => DeviceInformation.Pairing.IsPaired ? "Paired" : "Not Paired";
	}
}