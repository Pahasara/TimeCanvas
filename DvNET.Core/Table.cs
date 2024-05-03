using System.Data.SQLite;

namespace DvNET.Core
{
    public class Table
    {
        Database database = new Database();

        // Attributes
        private SQLiteConnection conn;
        private string name;
        private string attributes; //just an example attribute

        // Constructor
        public Table(SQLiteConnection conn, string name, string attributes)
        {
            this.conn = conn;        
            this.name = name;
            this.attributes = attributes;
        }

        // Properties (getters and setters)
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Attributes
        {
            get { return this.attributes; }
            set { this.attributes = value; }
        }

        // Methods
        public void Create()
        {
            string command = $"CREATE TABLE {this.name} ({this.attributes});";
            database.ExecuteSQL(conn, command);
        }

        public void Initialize(string attributes, string values)
        {
            string command = $"INSERT INTO {this.name} ({attributes}) VALUES ({values});";
            database.ExecuteSQL(conn, command);
        }

        public void Update(string values, string condition)
        {
            string command = $"UPDATE {this.name} SET {values} WHERE {condition};";
            database.ExecuteSQL(conn , command);
        }

        public void Delete()
        {
            string command = $"DROP TABLE {this.name};";
            database.ExecuteSQL(conn, command);
        }

        public string[] Search(string id)
        {
            return database.Search(conn, this.name, id);
        }
    }
}
