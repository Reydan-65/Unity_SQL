using System;
using System.Data.SQLite;

namespace Server
{
    public class DBConnection
    {
        private const string DataSource = "database.db";
        private SQLiteConnection connection;

        public void Open()
        {
            connection = new SQLiteConnection($"Data Source = {DataSource}");
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public void ExecuteCommand(string commandText)
        {
            SQLiteCommand command = new SQLiteCommand(commandText, connection);
            command.ExecuteNonQuery();
        }

        public SQLiteDataReader ExecuteCommandWithResult(string commandText)
        {
            SQLiteCommand command = new SQLiteCommand(commandText, connection);
            return command.ExecuteReader();
        }
    }
}
