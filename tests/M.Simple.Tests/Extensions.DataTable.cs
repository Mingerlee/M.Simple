using System.Data;
using Newtonsoft.Json;

namespace M.Simple.Tests
{
    [TestClass]
    public class ExtensionsDataTable
    {
        [TestMethod]
        public void TestToLists()
        {
            DataTable dt = new DataTable();
            if (dt.IsNullOrEmpty())
            {
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Code", typeof(string));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Title", typeof(string));
                dt.Columns.Add("Age", typeof(int));
                dt.Columns.Add("CreartDate", typeof(DateTime));
                dt.Columns.Add("CreartUser", typeof(string));
                dt.Columns.Add("Vaild", typeof(int));
                dt.Rows.Add(1, "C001", "����", "Teacher", 45, DateTime.Now, "U0001", 1);
                dt.Rows.Add(2, "C002", "����", "Student", 19, DateTime.Now, "U0001", 1);
                dt.Rows.Add(3, "C003", "����", "Student", 20, DateTime.Now, "U0001", 1);
                dt.Rows.Add(4, "C004", "½��", "Student", 20, DateTime.Now, "U0001", 1);
                dt.Rows.Add(5, "C005", "����", "Student", 19, DateTime.Now, "U0001", 1);

                var s = dt.Where(row => row["Code"].ToString()== "C001");
                Console.WriteLine(JsonConvert.SerializeObject(s));
                Console.WriteLine();

                var list = dt.ToLists<Person>();
                Console.WriteLine(JsonConvert.SerializeObject(list));
            }
        }
    }
}