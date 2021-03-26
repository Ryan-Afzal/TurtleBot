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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Bluetooth_UWP_Client {
    public sealed partial class MainPage : Page {

        private DeviceWatcher deviceWatcher;

        public MainPage() {
            this.InitializeComponent();

            this.deviceWatcher = null;
            DeviceList = new ObservableCollection<DeviceInformationViewModel>();
        }

		public ObservableCollection<DeviceInformationViewModel> DeviceList { get; set; }

		#region Events

		private void StartDeviceWatcherButton_Click(object sender, RoutedEventArgs e) {
            this.StartDeviceWatcher();
        }

        private void StopDeviceWatcherButton_Click(object sender, RoutedEventArgs e) {
            this.StopDeviceWatcher();
        }

        private void PairingToggle_Toggled(object sender, RoutedEventArgs e) {
            var toggleSwitch = (ToggleSwitch)sender;

            if (toggleSwitch.IsOn) {

			} else {

			}
        }

        #endregion Events

        private void StartDeviceWatcher() {
            StartDeviceWatcherButton.IsEnabled = false;
            DeviceList.Clear();

            this.deviceWatcher = DeviceInformation.CreateWatcher(
                BluetoothDevice.GetDeviceSelector(), 
                new string[] {
                    "System.Devices.Aep.DeviceAddress", 
                    "System.Devices.Aep.IsConnected"
                },
                DeviceInformationKind.AssociationEndpoint
                );
            this.deviceWatcher.Added += this.DeviceAdded;
            this.deviceWatcher.Removed += this.DeviceRemoved;
            this.deviceWatcher.Updated += this.DeviceUpdated;
            this.deviceWatcher.Start();

            StopDeviceWatcherButton.IsEnabled = true;
		}

        private void StopDeviceWatcher() {
            StopDeviceWatcherButton.IsEnabled = false;

            this.deviceWatcher.Stop();
            this.deviceWatcher = null;

            StartDeviceWatcherButton.IsEnabled = true;
        }

        private DeviceInformationViewModel FindById(string id) {
            return DeviceList.First((model) => model.Id == id);
		}

        private async void DeviceAdded(DeviceWatcher watcher, DeviceInformation info) {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                DeviceList.Add(new DeviceInformationViewModel() {
                    Id = info.Id,
                    DeviceInformation = info
                });
            });

            Debug.Write($"Device Added{Environment.NewLine}" +
                $" Name: {info.Name}{Environment.NewLine}" +
                $" Id: {info.Id}{Environment.NewLine}" +
                $" Kind: {info.Kind}{Environment.NewLine}" +
                $" Paired?: {info.Pairing?.IsPaired ?? false}{Environment.NewLine}" +
                $" Properties: {Environment.NewLine}"
                );

            foreach (var property in info.Properties) {
                Debug.Write($"  {property.Key}: {property.Value}{Environment.NewLine}");
            }

            Debug.WriteLine("");
        }

        private async void DeviceUpdated(DeviceWatcher watcher, DeviceInformationUpdate update) {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                var info = FindById(update.Id);
                info.DeviceInformation.Update(update);
            });
        }

        private async void DeviceRemoved(DeviceWatcher watcher, DeviceInformationUpdate update) {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                var info = FindById(update.Id);
                info.DeviceInformation.Update(update);

                DeviceList.Remove(info);
            });
        }
	}
}
