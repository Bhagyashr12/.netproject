using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Xianinfotech
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private int qty = 0;
        string barcode_k = string.Empty;
        string constring = "Data Source=LAPTOP-0LQP053P\\SQLEXPRESS01;Initial Catalog=LOGIN;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        //string constring = ConfigurationManager.ConnectionStrings["myDBconnection"].ConnectionString;
        public Form1()
        {
            InitializeComponent();
            time();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
        private void label6_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void time()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += timer1_Tick;
            timer.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lbltime.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy hh:mm:ss tt");
        }
        int currentQty;


        private void UpdateTotal()
        {
            double totalSum = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Total"].Value != null && !string.IsNullOrEmpty(row.Cells["Total"].Value.ToString()))
                {
                    if (double.TryParse(row.Cells["Total"].Value.ToString(), out double total))
                    {
                        totalSum += total;
                    }
                }
            }

            label10.Text = totalSum.ToString("0.00");
            double discount = totalSum == 0 ? 0 : 0.25;
            label9.Text = discount.ToString("0.00");

            double totalAfterDiscount = totalSum - discount;
            label8.Text = totalAfterDiscount.ToString("0.00");

            double percentageOfSubtotal = 14.45 / 100 * totalAfterDiscount;
            label7.Text = percentageOfSubtotal.ToString("0.00");
            double totalpayable = Math.Round(totalAfterDiscount + percentageOfSubtotal, 2);
            lblpaybel.Text = totalpayable.ToString("0.00");

        }

        private void UpdateRowTotal()
        {
            int rowCount = dataGridView1.Rows.Count - 1;
            lbliem.Text = rowCount.ToString();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            lbliem.Text = ""; label10.Text = ""; label7.Text = ""; label8.Text = ""; label9.Text = ""; lblpaybel.Text = "";
            dataGridView1.Rows.Clear();
        }
        string barcode1 = "";

        public string generateCode()
        {
            DateTime currentDate = DateTime.Now;
            string month = currentDate.ToString("MM");
            string day = currentDate.ToString("dd");
            string year = currentDate.ToString("yy");
            Random random = new Random();
            barcode1 = GenerateRandomBarcode(day, month, year, random);

            return barcode1;
        }

        private string GenerateRandomBarcode(string day, string month, string year, Random random)
        {
            StringBuilder randomBcode = new StringBuilder(4);
            for (int i = 0; i < 4; i++)
            {
                int num = random.Next(0, 10);
                randomBcode.Append(num);
            }
            return day + month + year + randomBcode.ToString();
        }

        private void lblpayment_Click(object sender, EventArgs e)
        {
            barcode_k = generateCode();
            string barcode = barcode_k;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                string connectionString = "Data Source=LAPTOP-0LQP053P\\SQLEXPRESS01;Initial Catalog=LOGIN;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

                string item = row.Cells["Items1"].Value.ToString();
                string price = row.Cells["Price"].Value.ToString();
                string qty = row.Cells["Qty1"].Value.ToString();

                string query = "INSERT INTO salesregi (BARCODES, ITEMS, PRICE, QTY, TOTAL,DISCOUNT, TAX, SUB_TOTAL, TOATALPAYABLE, TOTAL_ITEM) " +
                               "VALUES (@BARCODES, @ITEMS, @PRICE, @QTY, @TOTAL,@DISCOUNT, @TAX, @SUB_TOTAL, @TOATALPAYABLE, @TOTAL_ITEM)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BARCODES", barcode);
                        cmd.Parameters.AddWithValue("@ITEMS", item);
                        cmd.Parameters.AddWithValue("@PRICE", price);
                        cmd.Parameters.AddWithValue("@QTY", qty);
                        cmd.Parameters.AddWithValue("@TOTAL", label10.Text);
                        cmd.Parameters.AddWithValue("@DISCOUNT", label9.Text);
                        cmd.Parameters.AddWithValue("@TAX", label7.Text);
                        cmd.Parameters.AddWithValue("@SUB_TOTAL", label8.Text);
                        cmd.Parameters.AddWithValue("@TOATALPAYABLE", lblpaybel.Text);
                        cmd.Parameters.AddWithValue("@TOTAL_ITEM", lbliem.Text);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                lbliem.Text = ""; label10.Text = ""; label7.Text = ""; label8.Text = ""; label9.Text = ""; lblpaybel.Text = "";
                dataGridView1.Rows.Clear();
            }
        }

        private void picstabery_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "Strawberry 400gm";
            double Price = 1.25;
            double Tax = 0;
            string Comment = "Fresh Strawberries";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out int currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }

                        double Total = Price * currentQty;
                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated && e.Button == MouseButtons.Left)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }

            UpdateTotal();
            UpdateRowTotal();
        }

        private void picbur_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "HamBurger_SMALL";
            double Price = 3.99;
            double Tax = 0;
            string Comment = "............";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();

        }

        private void picdew_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "MountainDew355";
            double Price = 3;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();

        }

        private void picpiza_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "PIZZA_2KG";
            double Price = 66;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();
        }

        private void picpall_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "Pall_Mall_20_KING";
            double Price = 6.5;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();
        }

        private void picice_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "Ice_cream_330gm";
            double Price = 2.99;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();
        }

        private void picstaw_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "Juice_250gm";
            double Price = 1.99;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();
        }

        private void picbana_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "Banana_lb";
            double Price = 0.79;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();
        }

        private void picgrapes_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "Grapes_KG";
            double Price = 3.99;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();
        }

        private void picapple_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "Apple_400gm";
            double Price = 3;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();
        }

        private void piccoffe_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "Coffe_Mix_Milk";
            double Price = 2.49;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();

        }

        private void picpizzamid_MouseClick(object sender, MouseEventArgs e)
        {
            string De = "X";
            string Items = "Pizza_Mid400gm";
            double Price = 9.99;
            double Tax = 0;
            string Comment = "..........";

            bool itemUpdated = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Items1"].Value != null && row.Cells["Items1"].Value.ToString() == Items)
                {
                    if (row.Cells["Qty1"].Value != null && int.TryParse(row.Cells["Qty1"].Value.ToString(), out currentQty))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            currentQty++;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (currentQty >= 1)
                            {
                                currentQty--;
                            }
                            if (currentQty == 0)
                            {
                                dataGridView1.Rows.Remove(row);
                                UpdateTotal();
                                UpdateRowTotal();
                                return;
                            }
                        }
                        double Total = Price * currentQty;

                        row.Cells["Qty1"].Value = currentQty;
                        row.Cells["Total"].Value = Total.ToString("0.00");

                        itemUpdated = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity value!");
                    }
                }
            }

            if (!itemUpdated)
            {
                int qty = 1;
                double initialTotal = Price * qty;
                dataGridView1.Rows.Add(De, Items, qty, Price.ToString("0.00"), initialTotal.ToString("0.00"), Tax.ToString("0.00"), Comment);
            }
            UpdateTotal();
            UpdateRowTotal();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string barcode = textBox1.Text.Trim();

            if (barcode.Length == 10 && barcode.All(char.IsDigit))
            {
                FetchBarcodeData(barcode);
            }
            else
            {
                MessageBox.Show("Please enter a valid 10-digit barcode.");
            }
        }

        private void FetchBarcodeData(string barcode)
        {
            string connectionString = "Data Source=LAPTOP-0LQP053P\\SQLEXPRESS01;Initial Catalog=LOGIN;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            string query = "SELECT * FROM salesregi WHERE BARCODES = @BARCODES";
            string De = "X";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BARCODES", barcode);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string item = reader["ITEMS"].ToString();
                                    string price = reader["PRICE"].ToString();
                                    string qty = reader["QTY"].ToString();
                                    string total = reader["TOTAL"].ToString();

                                    dataGridView1.Rows.Add(De,item, price, qty, total);
                                    UpdateTotal();
                                    UpdateRowTotal();
                                }
                            }
                            else
                            {
                                MessageBox.Show("No data found for the given barcode.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error fetching data: {ex.Message}");
                    }
                }
            }
        }
    }
}
