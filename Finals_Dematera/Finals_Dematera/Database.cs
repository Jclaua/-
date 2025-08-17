using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Finals_Dematera
{
    internal class Database
    {
        private const string DB_HOST = "localhost";
        private const string DB_NAME = "bookings";
        private const string DB_USERNAME = "root";
        private const string DB_PASSWORD = "";
        private string ConnString = $"SERVER={DB_HOST};" +
                                    $"USER ID={DB_USERNAME};" +
                                    $"PASSWORD={DB_PASSWORD};" +
                                    $"DATABASE={DB_NAME};" + 
                                    $"Allow Zero Datetime = True;" +
                                    $"Convert Zero Datetime = True";

        public MySqlConnection DBConnection;
        public string TableName { get; set; }
        private string sqlQuery = "";
        public List<string> Columns = null;
        public List<string> Where { get; set; }
        public List<string> GroupBy { get; set; }
        public List<string> OrderBy { get; set; }
        public Database()
        {
            DBConnection = new MySqlConnection(ConnString);
        }
        public void OpenDB()
        {
            try
            {
                if (DBConnection == null || DBConnection.State != System.Data.ConnectionState.Open)
                {
                    DBConnection.Open();
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"ERROR: {ex.Message}");
            }
        }

        public DataTable GetRows()
        {
            string selectColumns = Columns != null && Columns.Count > 0 ? string.Join(", ", Columns) : "*";
            sqlQuery = $"SELECT {selectColumns} FROM {TableName}";

            if (Where != null && Where.Count > 0)
            {
                string whereClause = string.Join(" AND ", Where);
                sqlQuery += $" WHERE {whereClause}";
            }

            if (GroupBy != null && GroupBy.Count > 0)
            {
                string groupByClause = string.Join(", ", GroupBy);
                sqlQuery += $" GROUP BY {groupByClause}";
            }

            if (OrderBy != null && OrderBy.Count > 0)
            {
                string orderByClause = string.Join(", ", OrderBy);
                sqlQuery += $" ORDER BY {orderByClause}";
            }

            Debug.Print(sqlQuery);
            using (MySqlCommand cmd = new MySqlCommand(sqlQuery, DBConnection))
            {
                DataTable dt = new DataTable();

                try
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.Print($"MySQL ERROR: {ex.Message}");
                    this.CloseDB();
                }
                catch (Exception ex)
                {
                    Debug.Print($"ERROR: {ex.Message}");
                    this.CloseDB();
                }

                return dt;
            }
        }

        public DataTable GetRowsWithFilters(string eventType, DateTime date1, DateTime date2)
        {
            OpenDB();

            string setColumns = "*";

            string sqlQuery = $"SELECT {setColumns} FROM tblbookings WHERE eventype = @EventType AND eventdate BETWEEN @Date1 AND @Date2";

            Debug.Print($"Date1: {date1.ToString("yyyy-MM-dd")}, Date2: {date2.ToString("yyyy-MM-dd")}");

            using (MySqlCommand cmd = new MySqlCommand(sqlQuery, DBConnection))
            {
               
                cmd.Parameters.AddWithValue("@EventType", eventType);
                cmd.Parameters.AddWithValue("@Date1", date1.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@Date2", date2.ToString("yyyy-MM-dd"));

                DataTable dt = new DataTable();

                try
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dt.Columns.Add(reader.GetName(i));
                        }

                        while (reader.Read())
                        {
                            DataRow row = dt.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i];
                            }
                            dt.Rows.Add(row);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.Print($"MySQL ERROR: {ex.Message}");
            
                }
                catch (Exception ex)
                {
                    Debug.Print($"ERROR: {ex.Message}");
                    
                }
                finally
                {
                   
                    CloseDB();
                }

                return dt;
            }
        }

        public void CloseDB()
        {
            DBConnection.Close();
        }



        public string str_repl(string str)
        {
            return str.Replace("'", "").Replace(",", ",").Replace("`", "");
        }

        public T NotNull<T>(T Value, T DefaultValue)
        {
            if (Value == null || DBNull.Value.Equals(Value))
            {
                return DefaultValue;
            }
            else
            {
                return Value;
            }
        }

        public bool ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
        {
            using (MySqlCommand cmd = new MySqlCommand(sql, DBConnection))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                    }
                }

                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Debug.Print($"MySQL Error Code: {ex.ErrorCode}");
                    Debug.Print($"MySQL Error Message: {ex.Message}");
                    Debug.Print($"SQL Query: {cmd.CommandText}");

                    if (ex.ErrorCode == 1062)
                    {
                        MessageBox.Show("Duplicate key error: The Code already exists.");
                    }

                    this.CloseDB();
                    return false;
                }
            }
        }


    }
}
