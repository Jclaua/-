using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Finals_Dematera
{
    public partial class Form1 : Form
    {
        Database DB = new Database();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnNewBooking_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
        }
        private void LoadDataIntoDataGridView()
        {
            try
            {
                DB.OpenDB();
                DB.TableName = "tblbookings";
                List<string> col = new List<string>
        {
            "*"
        };
                DataTable dtbookings = new DataTable();
                dtbookings = DB.GetRows();

                dtbookings.DefaultView.Sort = "eventdate DESC";
                dtbookings = dtbookings.DefaultView.ToTable();


                Debug.Print($"Rows returned: {dtbookings.Rows.Count}");

                foreach (DataRow dr in dtbookings.Rows)
                {
                   
                    dataGridView1.Rows.Add(dr[0], dr[2], dr[8], dr[6], dr[3], dr[7], "Confirmed");
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error loading data: {ex.Message}");
            }
            finally
            {
           
                DB.CloseDB();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
        }

        private void btnRefData_Click(object sender, EventArgs e)
        {

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string code = selectedRow.Cells[0].Value.ToString();

            string sql = $"DELETE FROM tblbookings WHERE Code='{code}'";

            DB.OpenDB();
            if (DB.ExecuteQuery(sql))
            {
                MessageBox.Show("Product Successfully Deleted");
                dataGridView1.Rows.Remove(selectedRow);
            }
            else
            {
                MessageBox.Show("Delete failed");
            }
            DB.CloseDB();
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            string code = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

            Form2 form = new Form2();
            form.MODE = "EDIT";

            form.code = code;

            form.LoadDataForEdit(code);

            form.ShowDialog();
            dataGridView1.Rows.Clear();
            LoadDataIntoDataGridView();
        }

        private void txtsearch_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cbxEtype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtrefresh_Click(object sender, EventArgs e)
        {
            cbxEtype.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            dataGridView1.Rows.Clear();
            LoadDataIntoDataGridView();
        }

        private void txtsearch_Click_1(object sender, EventArgs e)
        {
        }
    }
}
