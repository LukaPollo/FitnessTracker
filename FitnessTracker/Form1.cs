using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace FitnessTracker
{
    public partial class Form1 : Form
    {
        private string adatbazisFajl = "C:\\Users\\explo\\Documents\\FitnessTracker\\FitnessTracker\\fitnessData.db";
        private string csvFajl = "C:\\Users\\explo\\Documents\\FitnessTracker\\FitnessTracker\\fitnessData.csv";

        public Form1()
        {
            InitializeComponent();
            Batteries_V2.Init();
        }

        private void Form1_Load(object sender, EventArgs e) 
        {
            dvgLoader();
        }



        private void SubmitBtn_Click(object sender, EventArgs e)
        {
            string sport = textBoxSport.Text;
            string date = textBoxDate.Text;
            string timeStr = textBoxTime.Text;
            string location = textBoxLocation.Text;

            if(!int.TryParse(timeStr, out int period))
            {
                MessageBox.Show("Hibas idotartam van adva");
                return;
            }

            using(var conn = new SqliteConnection("Data Source=" + adatbazisFajl))
            {
                conn.Open();
                var cmd = new SqliteCommand("INSERT INTO fitnessData (SportBranch, Date, Period, Location) VALUES (@sport, @date, @period, @location)", conn);
                cmd.Parameters.AddWithValue("@sport", sport);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@period", period);
                cmd.Parameters.AddWithValue("@location", location);

                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("sikeres felvitel");
            dvgLoader();
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            List<string> sorok = new List<string>();
            sorok.Add("SportBranch,Date,Period,Location");

            using (var conn = new SqliteConnection("Data Source=" + adatbazisFajl))
            {
                conn.Open();
                var cmd = new SqliteCommand("SELECT SportBranch, Date, Period, Location FROM fitnessData", conn);
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    string sport = reader.GetString(0);
                    string date = reader.GetString(1);
                    int period = reader.GetInt32(2);
                    string location = reader.GetString(3);

                    sorok.Add($"{sport},{date},{period},{location}");
                }
            }
            File.WriteAllLines(csvFajl, sorok); 
            MessageBox.Show("csv sikeres");
        }



        private void ImportBtn_Click(object sender, EventArgs e)
        {
            using (var conn = new SqliteConnection("Data Source=" + adatbazisFajl))
            {
                conn.Open();

                using (var cmd = new SqliteCommand(
                    "CREATE TABLE IF NOT EXISTS fitnessData (SportBranch TEXT, Date TEXT, Period INTEGER, Location TEXT)", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var reader = new StreamReader(csvFajl))
                {
                    reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(',');
                        using (var cmdInsert = new SqliteCommand(
                            "INSERT INTO fitnessData (SportBranch, Date, Period, Location) VALUES (@sport, @date, @period, @location)", conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@sport", parts[0].Trim());
                            cmdInsert.Parameters.AddWithValue("@date", parts[1].Trim());
                            int.TryParse(parts[2].Trim(), out int period);
                            cmdInsert.Parameters.AddWithValue("@period", period);
                            cmdInsert.Parameters.AddWithValue("@location", parts[3].Trim());

                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                }
            }
           
            MessageBox.Show("adatbazis sikeres");
        }

        private void dvgLoader()
        {
            using (var conn = new SqliteConnection("Data Source=" + adatbazisFajl))
            {
                conn.Open();
                var cmd = new SqliteCommand("SELECT SportBranch, Date, Period, Location FROM fitnessData", conn);
                var reader = cmd.ExecuteReader();
                var table = new System.Data.DataTable();
                table.Load(reader);

                dataViewGridMain.DataSource = table;
            }
        }

        private void dataViewGridMain_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void textBoxSport_TextChanged(object sender, EventArgs e) { }
        private void textBoxDate_TextChanged(object sender, EventArgs e) { }
        private void textBoxTime_TextChanged(object sender, EventArgs e) { }
        private void textBoxLocation_TextChanged(object sender, EventArgs e) { }

    }
}