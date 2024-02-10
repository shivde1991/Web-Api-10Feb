using Azure;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrations
{
    public class DapperContextDb: IDapperContextDb
    {
        private readonly IConfiguration _config;
        string  _connectionString;
        public DapperContextDb(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

       
        public async Task<IEnumerable<ReturnResponse>> SearchBooks(string key)
        {
            List<ReturnResponse> returnResponses = new List<ReturnResponse>();
            string sql;
            int recordNo = int.TryParse(key, out int n) ? Convert.ToInt32(key) * 10 : 0;
            sql = $"SELECT TOP {recordNo} * FROM [dbo].[Book] order by BookId";


            string sqlGet = $"SELECT TOP {recordNo} * FROM [dbo].[Book] order by BookId";
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    // Execute the query asynchronously
                    var tasks = Enumerable.Range(0, 1) //parallel queries
                        .Select(async _ =>
                        {
                            var result = await db.QueryAsync<ReturnResponse>(sql);
                            returnResponses.AddRange(result);
                        });

                    // Wait for all tasks to complete
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Handle exceptions
            }

            return returnResponses;
        }
    }
}

