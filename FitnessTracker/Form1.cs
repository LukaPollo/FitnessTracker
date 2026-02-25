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

        private void Form1_Load(object sender, EventArgs e) { }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            List<string> sorok = new List<string>();
            sorok.Add("SportBranch,Date,Period,Location");

            string sport = textBoxSport.Text.Trim();
            string date = textBoxDate.Text.Trim();
            string timeStr = textBoxTime.Text.Trim();
            string location = textBoxLocation.Text.Trim();

            if (!int.TryParse(timeStr, out int period))
            {
                MessageBox.Show("Hibás időtartam!");
                return;
            }

            sorok.Add(sport + "," + date + "," + period + "," + location);

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

        private void textBoxSport_TextChanged(object sender, EventArgs e) { }
        private void textBoxDate_TextChanged(object sender, EventArgs e) { }
        private void textBoxTime_TextChanged(object sender, EventArgs e) { }
        private void textBoxLocation_TextChanged(object sender, EventArgs e) { }
    }
}