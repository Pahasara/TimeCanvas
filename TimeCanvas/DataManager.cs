using DvNET.Core;
using System;
using System.Data.SQLite;

namespace TimeCanvas
{
    internal class DataManager
    {
        private SQLiteConnection conn = new SQLiteConnection("Data Source = .TimeCanvas.db; version = 3; New = True; Compress = True;");

        private short numberOfRows = 27 + 1; // number of rows in the UI + 1


        public void SetConnection(SQLiteConnection conn)
        {
            this.conn = conn;
        }

        public bool GetIsChecked(string value)
        {
            if (int.Parse(value) == 1)
            {
                return true;
            }
            return false;
        }

        public static TimeSpan GetTime(string time)
        {
            TimeSpan selectedTime;
            string[] timeParts = time.Split(':');
            int hours = int.Parse(timeParts[0]);
            int minutes = int.Parse(timeParts[1]);
            selectedTime = new TimeSpan(hours, minutes, 0); // Seconds are ignored

            return selectedTime;
        }

        public void CreateTables()
        {
            ManageWeekdayTables("CREATE");
        }
        public void InitializeTables()
        {
            ManageWeekdayTables("INITIALIZE");
        }

        public void ResetTables()
        {
            ManageWeekdayTables("RESET_ALL");
        }

        public void ResetSingleTable(string nameOfTable)
        {
            Table table = new Table(conn, nameOfTable, "");
            for (int id = 1; id <= numberOfRows; id++)
            {
                table.Update("time='00:00:00', task='', isChecked=0", $"id={id}");
            }
        }

        public void ManageWeekdayTables(string operation)
        {
            string[] weekdays = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            foreach (string day in weekdays)
            {
                Table table = new Table(conn, day, "id INTEGER PRIMARY KEY NOT NULL, time VARCHAR(20), task VARCHAR(64), isChecked INTEGER");

                switch (operation)
                {
                    case "CREATE":
                        table.Create();
                        break;
                    case "INITIALIZE":
                        for (int id = 1; id <= numberOfRows; id++)
                        {
                            table.Initialize("id, time, task, isChecked", $"{id}, '00:00:00', '', 0");
                        }
                        break;
                    case "RESET_ALL":
                        for (int id = 1; id <= numberOfRows; id++)
                        {
                            table.Update("time='00:00:00', task='', isChecked=0", $"id={id}");
                        }
                        break;
                }
            }
        }
    }
}
