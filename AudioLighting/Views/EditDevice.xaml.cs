using AudioLighting.Models;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AudioLighting.Views
{
    /// <summary>
    /// Interaktionslogik für NewDevice.xaml
    /// </summary>
    public partial class EditDevice : Window
    {
        private UdpDevice toEdit;
        private readonly MainWindow mainWindowInstance;
        private readonly string initialName;
        public EditDevice(UdpDevice u, MainWindow x)
        {
            InitializeComponent();
            toEdit = u;
            initialName = string.Copy(u.DeviceName);
            RefreshEditFields();
            mainWindowInstance = x;
        }

        public EditDevice(UdpDevice u)
        {
            InitializeComponent();
            toEdit = u;
            initialName = string.Copy(u.DeviceName);
            RefreshEditFields();
        }

        private void RefreshEditFields()
        {
            txtName.Text = toEdit.DeviceName;
            txtIp.Text = toEdit.Ip;
            txtOutput.Text = "";
            nudLines.Value = toEdit.Lines;
            nudPort.Value = toEdit.Port;
            sldSmoothing.Value = toEdit.Smoothing;
        }

        private void BtnRestore_Click(object sender, RoutedEventArgs e)
        {
            toEdit = MyUtils.UdpDevices.Find(x => x.DeviceName == initialName).DeepCopy();
            RefreshEditFields();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyUtils.UdpDevices.Remove(MyUtils.UdpDevices.Find(x => x.DeviceName == initialName));
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when deleting the device:\n\n\nError message:\n" + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckIP())
            {
                return;
            }
            try
            {
                var toSet = MyUtils.UdpDevices.Find(x => x.DeviceName == initialName);
                MyUtils.UdpDevices.Remove(toSet);
                toSet = new UdpDevice(txtName.Text, txtIp.Text, (int)nudPort.Value, (int)nudLines.Value, (int)sldSmoothing.Value);
                MyUtils.UdpDevices.Add(toSet);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when saving the device:\n\n\nError message:\n" + ex.Message);
            }
        }

        public bool CheckIP()
        {
            return MyUtils.ValidateIp(txtIp.Text);
        }

        private void TxtIp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!CheckIP())
            {
                txtIp.BorderBrush = Brushes.Red;
                txtIp.BorderThickness = new Thickness(2);
                btnSave.IsEnabled = false;
                btnTestConnection.IsEnabled = false;
            }
            else
            {
                txtIp.BorderThickness = new Thickness(0);
                btnSave.IsEnabled = true;
                btnTestConnection.IsEnabled = true;
            }
        }

        private void BtnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckIP())
            {
                return;
            }

            if (MyUtils.IpReachable(txtIp.Text))
            {
                MessageBox.Show("Sucess!\n\nDevice is reachable.");
            }
            else
            {
                MessageBox.Show("Device could not be pinged!");
            }
        }

        private void BtnRequest_Click(object sender, RoutedEventArgs e)
        {
            var addr = "http://" + txtIp.Text + "/all";
            var response = "";
            try
            {
                using (var webClient = new WebClient())
                {
                    response = webClient.DownloadString(addr);
                }
                txtOutput.Text = response;
            }
            catch { txtOutput.Text = "Error!"; }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mainWindowInstance != null)
            {
                mainWindowInstance.RefreshDeviceList();
            }
        }
    }
}
