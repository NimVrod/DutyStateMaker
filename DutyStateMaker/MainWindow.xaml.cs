using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;

namespace DutyStateMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 1);
            LoadSettings();
        }

        public class Settings
        {
            public string Username { get; set; }
            public string Duty { get; set; }
            public string Timezone { get; set; }
        }

        public class Duty
        {
            public String Start { get; set; }
            public DateTime StartDate { get; set; }
            public String End { get; set; }
            public DateTime EndDate { get; set; }
            public String Proof { get; set; }
            public String Notes { get; set; }

        }

        Duty on_going_duty = new Duty();
        Settings settings = new Settings();

        public void UpdateSettingsLabel()
        {
            Username.Text = settings.Username;
            Duties.Text = settings.Duty;
            Timezone.Text = settings.Timezone;
        }

        public void LoadSettings()
        {
            //check if file settings.json exists if does load settings
            if (System.IO.File.Exists("settings.json"))
            {
                string json = System.IO.File.ReadAllText("settings.json");
                settings = JsonSerializer.Deserialize<Settings>(json);
            }
            else
            {
                //if not create settings.json and load settings
                System.IO.File.Create("settings.json");
                settings.Username = "Username";
                settings.Duty = "Duty";
                settings.Timezone = "Timezone";
            }

            UpdateSettingsLabel();
        }

        public void SaveSettings()
        {
            string json = JsonSerializer.Serialize(settings);
            System.IO.File.WriteAllText(@"settings.json", json);
        }

        public void UpdateSettings()
        {
            settings.Username = Username.Text;
            settings.Duty = Duties.Text;
            settings.Timezone = Timezone.Text;
        }

        public void UpdateDuty()
        {
            on_going_duty.EndDate = DateTime.Now;
            on_going_duty.Notes = Notes.Text;
            on_going_duty.Proof = proof.Text;
            on_going_duty.Start = Tablist_start.Text;
            on_going_duty.End = Tablist_end.Text;

        }

        public String MakeDutyState(Settings settings, Duty duty)
        {
            if (duty.Notes == "Notes" || duty.Notes == "")
            {
                duty.Notes = "";
            }
            else
            {
                duty.Notes = "Notes: " + duty.Notes;
            }

            return $@"Username: {settings.Username}
Duty: {settings.Duty}
{duty.Proof}

Time Started: {duty.StartDate.ToString("hh:mm")} {settings.Timezone}
Tablist Started: {duty.Start}

Time Ended: {duty.EndDate.ToString("hh:mm")} {settings.Timezone}
Tablist Ended: {duty.End}

{duty.Notes}";

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var start = on_going_duty.StartDate;

            var diff = now - start;

            Time.Text = diff.ToString(@"hh\:mm\:ss");
        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            on_going_duty.StartDate = DateTime.Now;
            End.IsEnabled = true;
            Start.IsEnabled = false;
            Timer.Start();
        }

        private void End_Click(object sender, RoutedEventArgs e)
        {
            End.IsEnabled = false;
            Start.IsEnabled = true;
            Timer.Stop();

            UpdateDuty();
            UpdateSettings();

            var ds = MakeDutyState(settings, on_going_duty);
            Output.Text = ds;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Output.Text);
        }
    }
}
