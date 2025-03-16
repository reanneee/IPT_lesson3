using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace lecturevid2.DataAccess
{
    public class DatabaseHelper
    {
        private readonly string _connString;
        public DatabaseHelper(string connString)
        {
            _connString = connString;
        }
        public DataTable SelectQuery(string query)
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(_connString))
            {
                connection.Open();
                using(MySqlCommand command=new MySqlCommand(query, connection))
                {
                    using(MySqlDataAdapter adapter=new MySqlDataAdapter(command) )
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }
        //create ,update,delete
        public int ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(_connString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    return command.ExecuteNonQuery();//return
                }
            }
         
        }
    }
}
