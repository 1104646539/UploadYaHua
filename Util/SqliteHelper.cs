using ControlzEx.Standard;
using Serilog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using uploadyahua.Model;

namespace uploadyahua.Util
{
    public class SqliteHelper
    {
        //相对路径 - 推荐
        //ORM建库功能说明：建议不要加目录ORM没办法创建文件夹，如果加目录需要手动建文件夹
        public static string ConnectionString = @"DataSource=Results.fff";
        public static int pageSize = 10;
        public static SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
        {
            DbType = SqlSugar.DbType.Sqlite,
            ConnectionString = SqliteHelper.ConnectionString,
            IsAutoCloseConnection = true
        });



        public static void init()
        {
            try
            {
                db.DbMaintenance.CreateDatabase();
                db.CodeFirst.InitTables(typeof(TestResult),typeof(Result));
             
                initData();
            }
            catch (Exception ex)
            {
                Log.Debug($"自动建表失败 {ex}");
            }
        }

        private static void initData()
        {
           
        }


        public static TestResult InsertTestResult(TestResult testResult)
        {
            try
            {
              return db.InsertNav(testResult).Include(tr => tr.Result, new InsertNavOptions()
                {
                    OneToManyIfExistsNoInsert = true
                }).ExecuteReturnEntity();
            }
            catch (Exception ex)
            {
                Log.Information($"插入数据失败 {ex}");
            }
            return null;
        }
        public static Task<List<TestResult>> QueryTestResultsToday() {
            return db.Queryable<TestResult>().Includes(tr => tr.Result).OrderByDescending(it=>it.Id).ToListAsync();
        }
        public static Task<List<TestResult>> QueryTestResults(int page, int pageSize)
        {
            return db.Queryable<TestResult>().Includes(tr => tr.Result).OrderByDescending(it => it.Id).ToOffsetPageAsync(page,pageSize);
        }
        public static int UpdateTestResult(TestResult tr)
        {
            //db.UpdateNav(tr).Include(it => it.Result);
            return db.Updateable(tr).ExecuteCommand();
        }

        public static TestResult GetTestResultForId(int id){
            return db.Queryable<TestResult>().Includes(tr => tr.Result).Single(it => it.Id == id);
        }
    }
}
