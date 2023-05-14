using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using Microsoft.Office.Interop.Excel;

namespace RadProject
{
    public partial class Form1 : Form
    {

        string[] report_columns = new string[] { "title", "amount", "summary_price" };

        string[] ru_report_columns = new string[] { "Название товара", "Количество", "Сумма" };

        List<string[]> data = new List<string[]>();

        DataSet ds = new DataSet();
        System.Data.DataTable dt = new System.Data.DataTable();
        NpgsqlConnection con;

        public Form1(NpgsqlConnection con)
        {
            InitializeComponent();
            this.con = con;
            InitClients();
            //this.con = new NpgsqlConnection(
            //        "Server=localhost; Port=5432; Username=postgres; Password=2305; database=RadStore"
            //    );
            //con.Open();
            //InitClients();
        }

        public void InitClients() {
            string sql = "SELECT * FROM Client;";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, this.con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            checkedListBox1.Items.Add("Все");
            checkedListBox1.ItemCheck += ItemCheck;
            // dataGridView1.DataSource = dt;
            foreach (DataRow row in dt.Rows) {
                var cells = row.ItemArray;
                checkedListBox1.Items.Add(cells[1] + " " + cells[2]);
            }
        }

        private void ItemCheck(object sender, ItemCheckEventArgs e) {
            CheckedListBox lb = sender as CheckedListBox;
            if (e.Index == 0)
            {
                bool flag = e.NewValue == CheckState.Checked ? true : false;
                for (int i = 1; i < lb.Items.Count; i++)
                    lb.SetItemChecked(i, flag);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = @"SELECT (SELECT go.title FROM Goods go WHERE cg.goods_id = go.goods_id), sum(cg.amount) as amount, sum(cg.price) as summary_price FROM (
	                                SELECT cg1.goods_id, cg1.amount, cg1.price FROM Contract_goods cg1
	                                JOIN Contract ct ON ct.contract_id = cg1.contract_id
	                                WHERE ct.status = 'ready for shipment' and ct.client_id = ANY(:values) 
			                                and ct.register_date >= :start_date and ct.register_date <= :end_date
                                ) as cg
                                GROUP BY cg.goods_id;";

            List<int> values = new List<int>();
            foreach (int index in checkedListBox1.CheckedIndices)
                if (!(index is 0))
                    values.Add(index);


            NpgsqlCommand com = new NpgsqlCommand(sql, this.con);

            com.Parameters.AddWithValue("values", values.ToArray());

            NpgsqlParameter date1 = new NpgsqlParameter("start_date", NpgsqlTypes.NpgsqlDbType.Date);
            date1.Value = dateTimePicker1.Value.Date;
            com.Parameters.Add(date1);

            NpgsqlParameter date2 = new NpgsqlParameter("end_date", NpgsqlTypes.NpgsqlDbType.Date);
            date2.Value = dateTimePicker2.Value;
            com.Parameters.Add(date2);

            NpgsqlDataReader reader = com.ExecuteReader();
            while (reader.Read()) {
                string str = "";
                //MessageBox.Show(el);
                string[] els = new string[report_columns.Length];
                for (int i = 0; i < report_columns.Length; i++) {
                    string el = reader[report_columns[i]].ToString();
                    els[i] = el;
                }
                data.Add(els);
            }
            reader.Close();

            make_report();
        }

        private void make_report() {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string filename = ofd.FileName;
            Microsoft.Office.Interop.Excel.Application excelObject = new Microsoft.Office.Interop.Excel.Application();
            excelObject.Visible = true;
            Workbook wb = excelObject.Workbooks.Open(filename, 0, false, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            Worksheet wsh = wb.Sheets[1];
            wsh.Columns.AutoFit();

            for (int i = 0; i < ru_report_columns.Length; i++) {
                wsh.Cells[1, i + 1] = ru_report_columns[i];
            }

            for (int i = 0; i < data.Count; i++) {
                for (int j = 0; j < report_columns.Length; j++) {
                    wsh.Cells[i + 2, j + 1] = data[i][j];
                }
            }
            
            wb.Save();
            wb.Close();
        }
    }
}
