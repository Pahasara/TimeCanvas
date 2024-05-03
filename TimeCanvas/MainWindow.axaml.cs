using Avalonia.Controls;
using Avalonia.Interactivity;
using DvNET.Core;
using System;
using System.Data.SQLite;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Media;

namespace TimeCanvas
{
    public partial class MainWindow : Window
    {
        SQLiteConnection conn = new SQLiteConnection("Data Source = TimeCanvas.db; version = 3; New = True; Compress = True;");
        DataManager dataManager = new DataManager();
        private string dayOfWeek = DateTime.Now.DayOfWeek.ToString();
        private string selectedDay;

        public MainWindow()
        {
            InitializeComponent();
            selectedDay = dayOfWeek;
            Update_Buttons();
            dataManager.SetConnection(conn);
            Load();
            StartClocks();
        }

        private void StartClocks()
        {
            new DispatcherTimer(TimeSpan.FromMicroseconds(100000), DispatcherPriority.Normal, TimerTick).Start();
        }

        void TimerTick(object sender, EventArgs e)
        {
            Update();
            ShowProgress();
        }

        private void DayButton_OnClick(object sender, RoutedEventArgs e)
        {
            var clickedButton = (Button)sender;

            string buttonText = clickedButton.Content.ToString();
            selectedDay = buttonText;
            Update_Buttons();
            Load();
        }

        private void Update_Buttons()
        {
            Button[] buttons = [ btnSunday, btnMonday, btnTuesday, btnWednesday, btnThursday, btnFriday, btnSaturday ];
            Button clickedButton = buttons[0];

            foreach (Button button in buttons)
            {
                button.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 28));
                if (button.Content.ToString() == selectedDay)
                { 
                    clickedButton = button; 
                }
            }
            clickedButton.Background = new SolidColorBrush(Color.FromArgb(255,23,140,225));
        }

        public void Initiate(object sender, RoutedEventArgs e)
        {
            dataManager.CreateTables();
            dataManager.InitializeTables();
        }

        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Load()
        {
            ManageControls("LOAD");
        }

        private void Update()
        {
            ManageControls("UPDATE");
        }

        private void RESET(object sender, RoutedEventArgs e)
        {
            ManageControls("RESET");
        }

        private void RESET_ALL(object sender, RoutedEventArgs e)
        {
            ManageControls("RESET_ALL");
            Load();
        }

        private void ShowProgress()
        {
            int completedTasks = 0;
            int totalTasks = 0;
            CheckBox[] checkBoxes =
            [
                checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9,
                checkBox10, checkBox11, checkBox12, checkBox13, checkBox14, checkBox15, checkBox16, checkBox17, checkBox18
            ];

            foreach(CheckBox checkBox in checkBoxes)
            {
                if (checkBox.IsChecked == true)
                {
                    completedTasks++;
                }
            }

            TextBox[] txtTasks =
            [
                txtTask1, txtTask2, txtTask3, txtTask4, txtTask5, txtTask6, txtTask7, txtTask8, txtTask9,
                txtTask10, txtTask11, txtTask12, txtTask13, txtTask14, txtTask15, txtTask16, txtTask17, txtTask18
            ];

            foreach (TextBox textBox in txtTasks)
            {
                if (textBox.Text != "")
                {
                    totalTasks++;
                }
            }

            for(int i = 0; i < 18; i++)
            {
                ValidCheckBox(checkBoxes[i], txtTasks[i]);
            }
            
            float percentage = Arithmetic.GetPercentage(completedTasks, (float)totalTasks);
            progressBar1.Value = percentage;
            lblProgress.Content = ((int)percentage).ToString() + " %";
            lblCount.Content = $"{completedTasks} / {totalTasks}";
        }

        private void ValidCheckBox(CheckBox checkBox, TextBox txtBox)
        {
            if(txtBox.Text != "")
                checkBox.IsEnabled = true;
            else
                checkBox.IsEnabled = false;
        }

        private void ManageControls(string operation)
        {
            TimePicker[] timePickers =
            [
                timePicker1, timePicker2, timePicker3, timePicker4, timePicker5, timePicker6, timePicker7, timePicker8, timePicker9,
                timePicker10, timePicker11, timePicker12, timePicker13, timePicker14, timePicker15, timePicker16, timePicker17, timePicker18
            ];

            TextBox[] txtTasks =
            [
                txtTask1, txtTask2, txtTask3, txtTask4, txtTask5, txtTask6, txtTask7, txtTask8, txtTask9,
                txtTask10, txtTask11, txtTask12, txtTask13, txtTask14, txtTask15, txtTask16, txtTask17, txtTask18
            ];

            CheckBox[] checkBoxes =
            [
                checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9,
                checkBox10, checkBox11, checkBox12, checkBox13, checkBox14, checkBox15, checkBox16, checkBox17, checkBox18
            ];

            for (int id = 1; id <= 18; id++)
            {
                int i = id - 1; // correct index counting
                Table table = new Table(conn, selectedDay, "");
                switch (operation)
                {
                    case "LOAD":
                        string[] column = table.Search(id.ToString());
                        TimeSpan loadedTime = DataManager.GetTime(column[1]);
                        timePickers[i].SelectedTime = loadedTime;
                        txtTasks[i].Text = column[2];
                        checkBoxes[i].IsChecked = dataManager.GetIsChecked(column[3]);
                        break;

                    case "UPDATE":
                        string selectedTime = timePickers[i].SelectedTime.ToString().Substring(0, 5);
                        int isChecked = (bool)checkBoxes[i].IsChecked ? 1 : 0;
                        string task = txtTasks[i].Text;
                        string command = $"time = '{selectedTime}', task = '{task}', isChecked = {isChecked}";
                        table.Update(command, $" id = {id}");
                        break;

                    case "RESET":
                        timePickers[i].SelectedTime = DataManager.GetTime("00:00:00");
                        txtTasks[i].Text = "";
                        checkBoxes[i].IsChecked = false;
                        Update();
                        break;

                    case "RESET_ALL":
                        dataManager.ResetTables();
                        break;
                }
            }
        }
    }
}