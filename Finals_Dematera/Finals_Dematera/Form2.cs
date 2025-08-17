using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Finals_Dematera
{
    public partial class Form2 : Form
    {

        public string MODE = "ADD";

        public string code = "";
        Database DB = new Database();
        public Form2()
        {
            InitializeComponent();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            DB.OpenDB();
            DB.TableName = "tblbookings";
            List<string> col = new List<string>
            {
                "*"
            };

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string eventName = txtEventName.Text;
            string eventDate = dtpEventDate.Value.Date.ToString("yyyy-MM-dd");
            int numGuest;
            string preferenaces = txtPreferenaces.Text;
            string phoneNum = txtphonenum.Text;
            string time = txtTime.Text;
            string customer = txtCustomer.Text;


            if (!int.TryParse(txtnumguest.Text, out numGuest) || numGuest < 0)
            {
                MessageBox.Show(string.Empty, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = "";

            DB.OpenDB();


            switch (MODE)
            {
                case "ADD":
                    sql = $@"INSERT INTO tblbookings (eventype, eventname, eventdate, numguest, preferenaces, phonenum, time, customer)
               VALUES ('{cbxEventType.Text}', '{eventName}', '{eventDate}', {numGuest}, '{preferenaces}', '{phoneNum}', '{time}', '{customer}')";
                    break;

                case "EDIT":
                    sql = $@"UPDATE tblbookings SET
                    eventype = '{cbxEventType.Text}',
                    eventname = '{eventName}',
                    eventdate = '{eventDate}',
                    numguest = {numGuest},
                    preferenaces = '{preferenaces}',
                    phonenum = '{phoneNum}',
                    time = '{time}',
                    customer = '{customer}'
                WHERE Code = '{code}'";
                    break;


                default:
                    MessageBox.Show("Invalid");
                    break;
            }

            if (DB.ExecuteQuery(sql))
            {
                MessageBox.Show(MODE == "ADD" ? "Added Successfuly" : "Booking Updated Successfully");
            }
            else
            {
                MessageBox.Show("Operation failed");
            }


            DB.CloseDB();
        }
        public void LoadDataForEdit(string code)
        {
            List<string> col = new List<string>
    {
        "*"
    };
            DB.OpenDB();
            DB.TableName = "tblbookings";
            DB.Columns = col;

            List<string> cond = new List<string>();
            cond.Add($"Code='{code}'");
            DB.Where = cond;

            DataTable dtBooking = new DataTable();
            dtBooking = DB.GetRows();

            if (dtBooking.Rows.Count > 0)
            {
                string eventType = dtBooking.Rows[0]["eventype"].ToString();
                string eventName = dtBooking.Rows[0]["eventname"].ToString();
                DateTime eventDate = Convert.ToDateTime(dtBooking.Rows[0]["eventdate"]);
                int guest = Convert.ToInt32(dtBooking.Rows[0]["numguest"]);
                string pref = dtBooking.Rows[0]["preferenaces"].ToString();
                string phoneNum = dtBooking.Rows[0]["phonenum"].ToString();
                string time = dtBooking.Rows[0]["time"].ToString();
                string customer = dtBooking.Rows[0]["customer"].ToString();

                cbxEventType.Text = eventType;
                txtEventName.Text = eventName;
                dtpEventDate.Value = eventDate;
                txtnumguest.Text = guest.ToString();
                txtPreferenaces.Text = pref;
                txtphonenum.Text = phoneNum;
                txtTime.Text = time;
                txtCustomer.Text = customer;
            }
            else
            {
                // Handle the case where the specified record is not found
                MessageBox.Show("Record not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DB.CloseDB();

        }
       


        private void txtPreferenaces_TextChanged(object sender, EventArgs e)
        {




        }

        private void txtnumguest_TextChanged(object sender, EventArgs e)
        {





        }
        private void txtnumguest_KeyPress(object sender, KeyPressEventArgs e)
        {







        }

        private void txtphonenum_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void txtTime_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void cbxEventType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
