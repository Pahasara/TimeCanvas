using Avalonia.Controls;
using Avalonia.Interactivity;
using DvNET.Core;
using System;
using System.Data.SQLite;
using Avalonia.Threading;

namespace TimeCanvas
{
    public partial class MainWindow : Window
    {
        SQLiteConnection conn = new SQLiteConnection("Data Source = TimeCanvas.db; version = 3; New = True; Compress = True;");
        DataManager dataManager = new DataManager();
        private string dayOfWeek = DateTime.Now.DayOfWeek.ToString();
        private string selectedDay;
        private DispatcherTimer _uiTimer, _updateTimer;
        private bool _isClocksRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Initiate(); // To initiate tables for the first time
            }
            catch 
            { 

            }
            finally
            {
                Boot();
            }
        }        

        private void Boot()
        {
            dataManager.SetConnection(conn);
            selectedDay = dayOfWeek;
            Load();
            InitializeClocks();
            StartClocks();
        }

        public void Initiate()
        {
            dataManager.CreateTables();
            dataManager.InitializeTables();
        }

        private void InitializeClocks()
        {
            _updateTimer = new DispatcherTimer(TimeSpan.FromMicroseconds(500000), DispatcherPriority.Normal, TimerTick);
            _uiTimer = new DispatcherTimer(TimeSpan.FromMicroseconds(100000), DispatcherPriority.Normal, ControlsTimerTick);
        }

        private void StartClocks()
        {
            if (!_isClocksRunning)
            {
                _updateTimer.Start();
                _uiTimer.Start();
                _isClocksRunning = true;
            }
        }

        private void StopClocks()
        {
            if (_isClocksRunning)
            {
                _updateTimer.Stop();
                _uiTimer.Stop();
                _isClocksRunning = false;
            }
        }

        void TimerTick(object sender, EventArgs e)
        {
            Update();
        }

        void ControlsTimerTick(object sender, EventArgs e)
        {
            ShowProgress();
            Update_Controls();
        }

        private void DayButton_OnClick(object sender, RoutedEventArgs e)
        {
            var clickedButton = (Button)sender;
            string buttonText = clickedButton.Content.ToString();
            selectedDay = buttonText;
            Load();

            // Return to top of the scrollViewer
            scrollViewer1.PageUp();
            scrollViewer1.PageUp();
            scrollViewer1.PageUp();
        }

        private TimePicker[] GetTimePickers() {
            TimePicker [] timePickers =
            [
               timePicker1, timePicker2, timePicker3, timePicker4, timePicker5, timePicker6, timePicker7, timePicker8, timePicker9,
                timePicker10, timePicker11, timePicker12, timePicker13, timePicker14, timePicker15, timePicker16, timePicker17, timePicker18,
                timePicker19, timePicker20, timePicker21, timePicker22, timePicker23, timePicker24, timePicker25, timePicker26, timePicker27
            ];

            return timePickers;
        }

        private TextBox[] GetTextBoxes()
        {
            TextBox[] textBoxes =
            [
                txtTask1, txtTask2, txtTask3, txtTask4, txtTask5, txtTask6, txtTask7, txtTask8, txtTask9,
                txtTask10, txtTask11, txtTask12, txtTask13, txtTask14, txtTask15, txtTask16, txtTask17, txtTask18,
                txtTask19, txtTask20, txtTask21, txtTask22, txtTask23, txtTask24, txtTask25, txtTask26, txtTask27
            ];

            return textBoxes;
        }

        private CheckBox[] GetCheckBoxes()
        {
            CheckBox[] checkBoxes =
            [
                checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9,
                checkBox10, checkBox11, checkBox12, checkBox13, checkBox14, checkBox15, checkBox16, checkBox17, checkBox18,
                checkBox19, checkBox20, checkBox21, checkBox22, checkBox23, checkBox24, checkBox25, checkBox26, checkBox27
            ];

            return checkBoxes;
        }

        private void Update_Controls()
        {
            Button[] buttons = [btnSunday, btnMonday, btnTuesday, btnWednesday, btnThursday, btnFriday, btnSaturday];
            UIControl.UpdateButtons(buttons, selectedDay);

            TimePicker[] timePickers = GetTimePickers();
            TextBox[] txtTasks = GetTextBoxes();
            UIControl.UpdateTaskSlots(timePickers, txtTasks);
        }

        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Load()
        {
            TimePicker[] timePickers = GetTimePickers();
            TextBox[] txtTasks = GetTextBoxes();
            CheckBox[] checkBoxes = GetCheckBoxes();

            for (int id = 0; id < txtTasks.Length; id++)
            {
                int index = id + 1; // correct index counting
                Table table = new Table(conn, selectedDay, "");
                string[] column = table.Search(index.ToString());
                TimeSpan time = DataManager.GetTime(column[1]);
                timePickers[id].SelectedTime = time;
                txtTasks[id].Text = column[2];
                checkBoxes[id].IsChecked = dataManager.GetIsChecked(column[3]);
            }
        }

        private void Update()
        {
            TimePicker[] timePickers = GetTimePickers();
            TextBox[] txtTasks = GetTextBoxes();
            CheckBox[] checkBoxes = GetCheckBoxes();

            for (int id = 0; id < txtTasks.Length; id++)
            {
                int index = id + 1;
                Table table = new Table(conn, selectedDay, "");
                string time = timePickers[id].SelectedTime.ToString();
                string task = txtTasks[id].Text;
                int isChecked = (bool)checkBoxes[id].IsChecked ? 1 : 0;
                string attributes = $"time = '{time}', task = \"{task}\", isChecked = {isChecked}";
                string condition = $" id = {index}";
                table.Update(attributes, condition);
            }
        }

        private void RESET(object sender, RoutedEventArgs e)
        {
            CheckBox[] checkBoxes = GetCheckBoxes();
            for (int id = 0; id < checkBoxes.Length; id++)
            {
                checkBoxes[id].IsChecked = false;
                Update();
            }
        }

        private void CLEAR(object sender, RoutedEventArgs e)
        {
            TimePicker[] timePickers = GetTimePickers();
            TextBox[] txtTasks = GetTextBoxes();
            CheckBox[] checkBoxes = GetCheckBoxes();

            for (int id = 0; id < txtTasks.Length; id++)
            {
                timePickers[id].SelectedTime = DataManager.GetTime("00:00:00");
                txtTasks[id].Text = "";
                checkBoxes[id].IsChecked = false;
                Update();
            }
        }

        private void ShowProgress()
        {
            int completedTasks = 0;
            int totalTasks = 0;
            CheckBox[] checkBoxes = GetCheckBoxes();

            foreach (CheckBox checkBox in checkBoxes)
            {
                if (checkBox.IsChecked == true)
                {
                    completedTasks++;
                }
            }

            TextBox[] txtTasks = GetTextBoxes();

            foreach (TextBox textBox in txtTasks)
            {
                if (textBox.Text != "")
                {
                    totalTasks++;
                }
            }

            for(int i = 0; i < txtTasks.Length; i++)
            {
                ValidCheckBox(checkBoxes[i], txtTasks[i]);
            }
            
            float percentage = Arithmetic.GetPercentage(completedTasks, (float)totalTasks);
            progressBar1.Value = percentage;
            lblProgress.Content = ((int)percentage).ToString() + " %";
            lblCount.Content = $"({completedTasks} / {totalTasks})";
        }

        private void ValidCheckBox(CheckBox checkBox, TextBox txtBox)
        {
            if(txtBox.Text != "")
                checkBox.IsEnabled = true;
            else
                checkBox.IsEnabled = false;
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            StartClocks();
        }

        private void WindowDeactivated(object sender, EventArgs e)
        {
            StopClocks();
        }
    }
}