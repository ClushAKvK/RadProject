using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RadProject
{

    public partial class MainFrom : Form
    {

        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        NpgsqlConnection con;

        Dictionary<string, string[]> buttons_tables = new Dictionary<string, string[]>()
        {
            ["button1"] = new string[] { "Client", "Клиенты" },
            ["button2"] = new string[] { "Goods", "Товары" },
            ["button3"] = new string[] { "Contract", "Договоры" },
            ["button4"] = new string[] { "Contract_Goods", "О договорах" }
        };

        string current_table = "";

        public MainFrom()
        {
            InitializeComponent();
            //dataGridView1.AutoGenerateColumns = false;
            this.con = new NpgsqlConnection(
                    "Server=localhost; Port=5432; Username=postgres; Password=2305; database=RadStore"
                );
            con.Open();
        }

        private void update_view(string table)
        {
            if (current_table == "Client" || current_table == "Goods")
            {
                string sql = "SELECT * FROM " + current_table + ";";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, this.con);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridView1.DataSource = dt;
            }
            else if (current_table == "Contract") {
                string sql = @"SELECT ct.contract_id, cl.last_name as client, ct.pay_type, ct.status, ct.register_date, ct.total_price 
                                FROM Contract ct
                                JOIN Client cl ON ct.client_id = cl.client_id;";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, this.con);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridView1.DataSource = dt;
            }
            else if (current_table == "Contract_Goods")
            {
                string sql = @"SELECT cg.contract_goods_id, cg.contract_id, go.title as goods, cg.amount, cg.price 
                                FROM Contract_Goods cg
                                JOIN Goods go ON go.goods_id = cg.goods_id;";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, this.con);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridView1.DataSource = dt;
                dataGridView1.Columns["contract_goods_id"].DisplayIndex = 0;
            }

            dataGridView1.Sort(dataGridView1.Columns[current_table.ToLower() + "_id"], ListSortDirection.Ascending);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.current_table = buttons_tables["button1"][0];
            table_label.Text = buttons_tables["button1"][1];
            update_view(this.current_table);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.current_table = buttons_tables["button2"][0];
            table_label.Text = buttons_tables["button2"][1];
            update_view(this.current_table);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.current_table = buttons_tables["button3"][0];
            table_label.Text = buttons_tables["button3"][1];
            update_view(this.current_table);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.current_table = buttons_tables["button4"][0];
            table_label.Text = buttons_tables["button4"][1];
            update_view(this.current_table);
        }

        private void отчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1(this.con);
            form.ShowDialog();
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFrom form = new AddFrom(this.con, this.current_table);
            form.ShowDialog();
            update_view(current_table);
        }

        private void MainFrom_Load(object sender, EventArgs e)
        {

        }

        private void изменитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridView1.CurrentRow.Cells[current_table + "_id"].Value;
            //MessageBox.Show(id.ToString());
            UpdateForm form = new UpdateForm(this.con, this.current_table, id);
            form.ShowDialog();
            update_view(current_table);
        }

        private void удалитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridView1.CurrentRow.Cells[current_table + "_id"].Value;
            NpgsqlCommand com = new NpgsqlCommand("DELETE FROM " + current_table + " WHERE " + current_table + "_id = " + id + ";", this.con);
            com.Parameters.AddWithValue("id", id);
            com.ExecuteNonQuery();
            update_view(current_table);
        }
    }
}
