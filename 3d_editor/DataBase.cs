using System.Data.OleDb;
using System.Data;

namespace _3d_editor
{
    class DataBase
    {
        // private fields
        private readonly OleDbConnection Connection;

        // Constructors
        public DataBase(string connectionString)
        {
            Connection = new OleDbConnection(connectionString);
        }

        // public methods
        public void FillDataTable(DataTable table, string tableName)
        {
            string query = $"SELECT * FROM [{tableName}]";
            var adapter = new OleDbDataAdapter(query, Connection);
            adapter.Fill(table);
        }

        public void UpdateDataBase(DataTable table, string tableName)
        {
            try
            {
                string query = $"SELECT * FROM [{tableName}]";
                using (var adapter = new OleDbDataAdapter(query, Connection))
                using (var builder = new OleDbCommandBuilder(adapter))
                {
                    builder.QuotePrefix = "[";
                    builder.QuoteSuffix = "]";

                    builder.GetInsertCommand();
                    builder.GetUpdateCommand();
                    builder.GetDeleteCommand();

                    if (Connection.State != ConnectionState.Open)
                        Connection.Open();

                    adapter.Update(table);
                }
            }
            catch (OleDbException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Ошибка БД");
                table.RejectChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General error: {ex.Message}", "Ошибка");
                table.RejectChanges();
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                table.AcceptChanges();
            }
        }



        public List<string> GetTablesNames()
        {
            List<string> tableNames = [];
            Connection.Open();
            DataTable schema = Connection.GetSchema("Tables");
            foreach (DataRow row in schema.Rows)
            {
                if (row["TABLE_TYPE"].ToString() != "TABLE")
                {
                    continue;
                }

                tableNames.Add(row["TABLE_NAME"].ToString());
            }

            Connection.Close();
            return tableNames;
        }

        

    }
}
