using IniParser;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PngTuber.Pupper
{
    public class RootViewModel : ViewModelBase
    {
        private bool allowBlink = true;
        private bool blinking;
        private bool isTriggered;
        private string avatarPath;

        private bool listenGlobalHotkeys;

        private BitmapImage currentImage;
        private ExpressionViewModel selectedExpression;

        private string modelName;
        private int triggerThreshold = 33;
        private bool isMonitoring;
        private int volumeLevel;
        private MMDevice selectedDevice;
        private MMDeviceEnumerator deviceEnumerator;

        private WasapiCapture waveIn;

        private DispatcherTimer blinkTimer;

        private Random blinkRandom;

        private HubConnection hub;

        private Thickness ZeroThickness { get; }
            = new Thickness(0);

        public DelegateCommand StartMonitor { get; }
        public DelegateCommand StopMonitor { get; }
        public DelegateCommand LoadExpressionSet { get; }

        public RootViewModel()
        {
            this.LoadExpressionSet = new DelegateCommand(this.OnLoadExpressionSetFromFolder);
            this.StartMonitor = new DelegateCommand(this.OnStartMonitoringAsync);
            this.StopMonitor = new DelegateCommand(this.OnStopMonitoringAsync);
            this.blinkTimer = new DispatcherTimer();
            this.blinkTimer.Tick += this.BlinkTimer_Tick;

            this.blinkRandom = new Random();
            this.blinkTimer.Interval = TimeSpan.FromSeconds(2);
            this.blinkTimer.Start();
        }
        public async Task StartHub()
        {
            this.hub = new HubConnectionBuilder()
                .WithUrl("http://localhost:65000/puppethub")
                .Build();

            await this.hub.StartAsync();
        }

        public async void SendSignalRDebugMessage()
        {
            await this.hub.InvokeAsync("Debug", "Hello world");
        }

        
        public bool AllowBlink
        {
            get => this.allowBlink;
            set
            {
                if (this.allowBlink == value)
                    return;
                this.allowBlink = value;

                if(this.Blinking && !this.allowBlink)
                {
                    this.Blinking = false;
                }

                this.NotifyPropertyChanged();
            }
        }
        

        public bool ListenGlobalHotkeys
        {
            get => this.listenGlobalHotkeys;
            set
            {
                if (this.listenGlobalHotkeys == value)
                    return;
                this.listenGlobalHotkeys = value;
                this.NotifyPropertyChanged();
            }
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            if (!this.IsMonitoring || !this.AllowBlink)
            {
                return;
            }

            this.Blinking = !this.Blinking;

            var blinkIntervalMin = this.Blinking
                ? 0.1
                : 1;

            var blinkIntervalMax = this.Blinking
                ? 0.4
                : 4;

            var interval = blinkIntervalMin + (this.blinkRandom.NextDouble() * blinkIntervalMax);

            this.blinkTimer.Interval = TimeSpan.FromSeconds(interval);


        }

        public BitmapImage CurrentImage
        {
            get
            {
                return this.currentImage;
            }
            set
            {
                if (this.currentImage == value)
                {
                    return;
                }

                this.currentImage = value;
                this.NotifyPropertyChanged();
            }
        }

        public string AvatarPath
        {
            get => this.avatarPath;
            set
            {
                if (this.avatarPath == value)
                    return;
                this.avatarPath = value;
                this.NotifyPropertyChanged();
            }
        }


        public int TriggerThreshold
        {
            get
            {
                return this.triggerThreshold;
            }
            set
            {
                if (this.triggerThreshold == value)
                {
                    return;
                }

                this.triggerThreshold = value;
                this.NotifyPropertyChanged();
            }
        }


        public bool IsTriggered
        {
            get => this.isTriggered;
            set
            {
                if (this.isTriggered == value)
                    return;
                this.isTriggered = value;
                this.NotifyPropertyChanged();
                this.SendExpressionUpdate();
            }
        }

        public bool Blinking
        {
            get => this.blinking;
            set
            {
                if (this.blinking == value)
                    return;
                this.blinking = value;
                this.NotifyPropertyChanged();
                this.SendExpressionUpdate();
            }
        }

        public bool IsMonitoring
        {
            get
            {
                return this.isMonitoring;
            }
            set
            {
                if (this.isMonitoring == value)
                {
                    return;
                }

                this.isMonitoring = value;
                this.NotifyPropertyChanged();
            }
        }

        public int VolumeLevel
        {
            get
            {
                return this.volumeLevel;
            }
            set
            {
                if (this.volumeLevel == value)
                {
                    return;
                }

                this.volumeLevel = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged(nameof(this.IsTriggered));
            }
        }

        public MMDevice SelectedDevice
        {
            get
            {
                return this.selectedDevice;
            }
            set
            {
                if (this.selectedDevice == value)
                {
                    return;
                }

                this.selectedDevice = value;
                this.NotifyPropertyChanged();
            }
        }

        public ObservableCollection<MMDevice> Devices { get; }
            = new ObservableCollection<MMDevice>();

        public string ModelName
        {
            get
            {
                return this.modelName;
            }
            set
            {
                if (this.modelName == value)
                {
                    return;
                }

                this.modelName = value;
                this.NotifyPropertyChanged();
            }
        }


        public ExpressionViewModel SelectedExpression
        {
            get
            {
                return this.selectedExpression;
            }
            set
            {
                if (this.selectedExpression == value)
                {
                    return;
                }

                this.selectedExpression = value;
                this.NotifyPropertyChanged();
                this.SendExpressionUpdate();
            }
        }


        public ObservableCollection<ExpressionViewModel> Expressions { get; }
            = new ObservableCollection<ExpressionViewModel>();
        public WebView2 WebViewPreview { get; set; }

        public void Initialize()
        {
            this.deviceEnumerator = new MMDeviceEnumerator();
            this.Devices.Clear();

            foreach (var item in this.deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                this.Devices.Add(item);
            }

            try
            {
                var parser = new FileIniDataParser();
                var iniFile = parser.ReadFile("settings.ini");

                var section = iniFile["AvatarWindowSettings"];

                this.ListenGlobalHotkeys = bool.Parse(section["ListenGlobal"]);

                section = iniFile["Devices"];

                var lastUsedDeviceName = section["LastUsedDeviceName"];
                if (!string.IsNullOrEmpty(lastUsedDeviceName))
                {
                    this.SelectedDevice = this.Devices.FirstOrDefault(x => x.FriendlyName == lastUsedDeviceName);
                }

                this.TriggerThreshold = int.Parse(section["TriggerVolume"]);

                section = iniFile["Avatar"];

                var avatarPath = section["Path"];

                if (!string.IsNullOrEmpty(avatarPath))
                {
                    this.LoadAvatarFromDirectory(avatarPath);
                }
            }
            catch
            {
            }
        }

        private async Task OnStartMonitoringAsync()
        {
            if (this.IsMonitoring)
            {
                return;
            }

            if (!this.Expressions.Any())
            {
                System.Windows.MessageBox.Show("Please load an avatar first.");
                return;
            }

            if(this.SelectedDevice == null)
            {
                System.Windows.MessageBox.Show("Please select a microphone before starting.");
                return;
            }

            this.SaveSettings();

            try
            {
                this.IsMonitoring = true;

                this.waveIn = new WasapiCapture(this.SelectedDevice);
                this.waveIn.DataAvailable += this.Capture_DataAvailable;
                this.waveIn.StartRecording();
            }
            catch (Exception ex)
            {
                this.IsMonitoring = false;
            }

            try
            {
                await this.StartHub();
                this.WebViewPreview.Source = new System.Uri(@"http://localhost:65000/viewer");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Sorry - we couldn't start the monitoring. Details have been logged to Crashlog.txt.");
                File.WriteAllText($"Crashlog.txt", ex.ToString());
                await this.OnStopMonitoringAsync();
            }
        }

        private async Task OnStopMonitoringAsync()
        {
            if (!this.IsMonitoring)
            {
                return;
            }

            try
            {
                await this.hub.StopAsync();
                this.IsMonitoring = false;
                this.waveIn.StopRecording();
                this.waveIn.DataAvailable -= this.Capture_DataAvailable;

                this.waveIn.Dispose();
                this.waveIn = null;
            }
            catch (Exception ex)
            {
                File.WriteAllText($"Crashlog.txt", ex.ToString());
                System.Windows.MessageBox.Show("Sorry - we couldn't stop the monitoring. Please close and restart the program.");
            }
        }

        private void OnLoadExpressionSetFromFolder()
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "All Avatar Definitions (*.ava)|*.ava";

            var result = openFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            this.Expressions.Clear();
            this.SelectedExpression = null;

            var directory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

            this.LoadAvatarFromDirectory(directory);
        }


        public void LoadAvatarFromDirectory(string directory)
        {
            WebServerEngine.Instance.AssetRootPath = directory;
            var expressionSets = Directory.GetDirectories(directory);

            this.AvatarPath = directory;

            foreach (var item in expressionSets)
            {
                var eyesCloseMouthClose = LocateFilePath(item, "4_EC_MC");
                var eyesCloseMouthOpen = LocateFilePath(item, "3_EC_MO");
                var eyesOpenMouthClose = LocateFilePath(item, "2_EO_MC");
                var eyesOpenMouthOpen = LocateFilePath(item, "1_EO_MO");

                var expression = new ExpressionSetModel()
                {
                    Name = item.Split("\\").LastOrDefault(),
                    EyesCloseMouthClosePath = eyesCloseMouthClose,
                    EyesCloseMouthOpenPath = eyesCloseMouthOpen,
                    EyesOpenMouthClosePath = eyesOpenMouthClose,
                    EyesOpenMouthOpenPath = eyesOpenMouthOpen,
                };

                this.Expressions.Add(new ExpressionViewModel(expression));
            }

            this.SelectedExpression = this.Expressions.FirstOrDefault();
        }

        private static string LocateFilePath(string root, string avatarPart)
        {
            var fileFound = System.IO.Directory.GetFiles(root, $"{avatarPart}.*");

            // Search for APNG, GIF, PNG in that order.
            return fileFound.OrderBy(x => x).FirstOrDefault();
        }

        public void ResetExpression()
        {

        }

        public void Shutdown()
        {
            this.SaveSettings();
            this.OnStopMonitoringAsync();
            this.hub?.StopAsync();
        }

        private void SaveSettings()
        {
            var parser = new FileIniDataParser();
            var iniFile = parser.ReadFile("settings.ini");

            var section = iniFile["AvatarWindowSettings"];

            section["ListenGlobal"] = this.ListenGlobalHotkeys.ToString();

            section = iniFile["Devices"];

            section["LastUsedDeviceName"] = this.SelectedDevice.FriendlyName;

            section["TriggerVolume"] = this.TriggerThreshold.ToString();

            section = iniFile["Avatar"];
            section["Path"] = this.AvatarPath;

            parser.WriteFile("settings.ini", iniFile);
        }

        private void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            float max = 0;

            var buffer = new WaveBuffer(e.Buffer);

            for (var i = 0; i < e.BytesRecorded / 4; i++)
            {
                var sample = buffer.FloatBuffer[i];
                sample = sample < 0 ? -sample : sample;
                max = sample > max ? sample : max;
            }

            this.VolumeLevel = (int)(max * 100);

            this.IsTriggered = this.VolumeLevel >= this.TriggerThreshold;
        }

        private void SendExpressionUpdate()
        {
            if (!this.IsMonitoring)
            {
                return;
            }

            var texturePath = string.Empty;

            if (this.IsTriggered && this.Blinking)
            {
                texturePath = this.SelectedExpression?.EyesCloseMouthOpenPath;
            }
            else if (this.IsTriggered)
            {
                texturePath = this.SelectedExpression?.EyesOpenMouthOpenPath;
            }
            else if (this.Blinking)
            {
                texturePath = this.SelectedExpression?.EyesCloseMouthClosePath;
            }
            else
            {
                texturePath = this.SelectedExpression?.EyesOpenMouthClosePath;
            }

            if (string.IsNullOrEmpty(texturePath))
            {
                return;
            }

            var textureToSend = texturePath[(texturePath.LastIndexOf('\\') + 1)..];

            this.hub?.SendAsync("AnimationUpdate", new 
            {
                Texture = $"{this.SelectedExpression?.Name}/{textureToSend}",
                Emotion = this.SelectedExpression.Name,
                Blink = this.Blinking
            });
        }

        public void UpdateExpressionIndex(int index, bool relative = false)
        {
            if (!this.Expressions.Any())
            {
                return;
            }

            if (relative)
            {
                var indexOfCurrent = this.Expressions.IndexOf(this.SelectedExpression);
                if(indexOfCurrent == -1)
                {
                    this.SelectedExpression = this.Expressions.FirstOrDefault();
                }
                else
                {
                    indexOfCurrent += index;

                    if(indexOfCurrent >= this.Expressions.Count)
                    {
                        this.SelectedExpression = this.Expressions.FirstOrDefault();
                    }
                    else if(indexOfCurrent < 0)
                    {
                        this.SelectedExpression = this.Expressions.LastOrDefault();
                    }
                    else
                    {
                        this.SelectedExpression = this.Expressions[indexOfCurrent];
                    }
                }
            }
            else
            {
                if (index >= this.Expressions.Count)
                {
                    this.SelectedExpression = this.Expressions.LastOrDefault();
                }
                else
                {
                    this.SelectedExpression = this.Expressions[index];
                }
            }
        }
    }
}
