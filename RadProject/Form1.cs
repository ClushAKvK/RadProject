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

namespace RadProject
{
    public partial class Form1 : Form
    {

        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
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
            string sql = @"SELECT cl.first_name, cl.last_name, gd.title, ct.amount, ct.total_price FROM Contract ct
                            JOIN Client cl ON cl.client_id = ct.client_id
                            JOIN Goods gd ON gd.goods_id = ct.goods_id
                            WHERE ct.status = 'ready for shipment' and ct.client_id = ANY(:values) 
	                            and ct.register_date >= :start_date and ct.register_date <= :end_date;";

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
                var el = reader["first_name"].ToString();
                MessageBox.Show(el);
            }
            reader.Close();
        }
    }
}
