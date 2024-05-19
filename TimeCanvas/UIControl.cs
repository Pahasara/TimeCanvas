using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace TimeCanvas
{
    public class UIControl
    {
        public static void UpdateButtons(Button[] buttons, string selectedDay)
        {
            foreach (Button button in buttons)
            {
                button.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 28));
                button.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 36, 36, 36));
                if (button.Content.ToString() == selectedDay)
                {
                    button.Background = new SolidColorBrush(Color.FromArgb(255, 23, 140, 225));
                    button.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 102, 153));
                }
            }
        }

        public static void UpdateTaskSlots(TimePicker[] timePickers, TextBox[] txtTasks)
        {
            for (int i = 0; i < timePickers.Length; i++)
            {
                TimeSpan time = DataManager.GetTime("00:00:00");
                if (timePickers[i].SelectedTime == time && txtTasks[i].Text == "")
                {
                    timePickers[i].Foreground = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
                    txtTasks[i].BorderBrush = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48));
                    timePickers[i].BorderBrush = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48));
                    timePickers[i].Background = new SolidColorBrush(Color.FromArgb(255, 12, 12, 12));
                    txtTasks[i].Background = new SolidColorBrush(Color.FromArgb(255, 12, 12, 12));
                }
                else
                {
                    timePickers[i].Foreground = new SolidColorBrush(Color.FromArgb(255, 249, 249, 249));
                    txtTasks[i].BorderBrush = new SolidColorBrush(Color.FromArgb(255, 50, 82, 98));
                    timePickers[i].BorderBrush = new SolidColorBrush(Color.FromArgb(255, 50, 82, 98));
                    timePickers[i].Background = new SolidColorBrush(Color.FromArgb(255, 36, 36, 36));
                    txtTasks[i].Background = new SolidColorBrush(Color.FromArgb(255, 36, 36, 36));
                }
            }
        }
    }
}
