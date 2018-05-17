using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using pisa.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace pisa.Controllers
{
    public class QueryController : ApiController
    {
        [HttpPost]
        public IList<Employee> GetEmployee([FromBody] QueryInfo queryInfo)
        {
            var session = HttpContext.Current.Session;
            if (null == session || null == session["user"]) throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var sql = "";
            if ( "name" == queryInfo.type )
            {
                sql = string.Format("SELECT * FROM employee WHERE name = '{0}'", queryInfo.query);
            }
            else
            {
                sql = string.Format("SELECT * FROM employee WHERE age = {0}", queryInfo.query);
            }

            var result = new List<Employee>();
            if ( "mysql" == queryInfo.db )
            {
                using (var conn = new MySqlConnection(ConfigurationManager.AppSettings["mysqlUrl"]))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var emp = new Employee
                        {
                            name = reader.GetString("Name"),
                            age = reader.GetInt32("Age")
                        };
                        result.Add(emp);
                    }
                }
            }
            else if ( "pgsql" == queryInfo.db )
            {
                using (var conn = new NpgsqlConnection(ConfigurationManager.AppSettings["pgsqlUrl"]))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var emp = new Employee
                        {
                            name = Convert.ToString(reader["Name"]),
                            age = Convert.ToInt32(reader["Age"])
                        };
                        result.Add(emp);
                    }
                }
            }
            else if ( "sqlserver" == queryInfo.db )
            {
                using (var conn = new SqlConnection(ConfigurationManager.AppSettings["sqlserverUrl"]))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        var emp = new Employee
                        {
                            name = Convert.ToString(reader["Name"]),
                            age = Convert.ToInt32(reader["Age"])
                        };
                        result.Add(emp);
                    }
                }
            }
            else if ( "oracle" == queryInfo.db )
            {
                using (var conn = new OracleConnection(ConfigurationManager.AppSettings["oracleUrl"]))
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(sql, conn);
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var emp = new Employee
                        {
                            name = Convert.ToString(reader["Name"]),
                            age = Convert.ToInt32(reader["Age"])
                        };
                        result.Add(emp);
                    }
                }
            }
            return result;
        }
    }
}
