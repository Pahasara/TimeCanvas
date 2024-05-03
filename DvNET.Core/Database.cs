using System.Data.SQLite;

namespace DvNET.Core
{
    public class Database
    {
        public void ExecuteSQL(SQLiteConnection conn, string command)
        {
            SQLiteCommand cmd;
            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandText = command;
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public int GetNumberOfRows(SQLiteConnection conn, string table, string id)
        {
            SQLiteDataReader reader;
            SQLiteCommand cmd;
            int numberOfRows = 0;

            cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT COUNT({id}) FROM {table};";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                numberOfRows = reader.GetInt32(0);
            }

            return numberOfRows;
        }

        public string[] GetIndexArray(SQLiteConnection conn, string table, string id)
        {
            SQLiteDataReader reader;
            SQLiteCommand cmd;
            int numberOfRows = GetNumberOfRows(conn, table, id);
            string[] indexes = new string[numberOfRows];
            int count = 0; // to set indexes[count]

            cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT {id} FROM {table} ORDER BY {id};";
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    indexes[count] = reader.GetString(0);
                    count++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing SQL query: {ex.Message}");
            }
            conn.Close();
            return indexes;
        }

        public string[] Search(SQLiteConnection conn, string table, string id)
        {
            SQLiteDataReader reader;
            SQLiteCommand cmd;
            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {table} WHERE id = {id};";

            string[] column = new string[4]; // Array size = number of Columns

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                column[0] = reader.GetInt32(0).ToString();
                column[1] = reader.GetString(1);
                column[2] = reader.GetString(2);
                column[3] = reader.GetInt32(3).ToString();
            }
            conn.Close();
            return column;
        }
    }
}