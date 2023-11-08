﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Data.SqlClient;
using System.Data;

namespace Sovelluskehitys_esimerkki
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k2101792\\Documents\\tuotekanta.mdf;Integrated Security=True;Connect Timeout=30";
        public MainWindow()
        {
            InitializeComponent();

            paivitaComboBox();

            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
        }

        private void painike_hae_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
            }
            catch 
            {
                viestirivi.Text = "Tietojen haku epäonnistui";
            }
        }

        private void painike_lisaa_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            string sql = "INSERT INTO tuotteet (nimi, hinta) VALUES ('" + tuote_nimi.Text + "','" + tuote_hinta.Text + "')";

            SqlCommand komento = new SqlCommand(sql, kanta);
            komento.ExecuteNonQuery();

            kanta.Close();

            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
            paivitaComboBox();
        }

        private void paivitaDataGrid(string kysely, string taulu, DataGrid grid)
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            /*tehdään sql komento*/
            SqlCommand komento = kanta.CreateCommand();
            komento.CommandText = kysely; // kysely

            /*tehdään data adapteri ja taulu johon tiedot haetaan*/
            SqlDataAdapter adapteri = new SqlDataAdapter(komento);
            DataTable dt = new DataTable(taulu);
            adapteri.Fill(dt);

            /*sijoitetaan data-taulun tiedot DataGridiin*/
            grid.ItemsSource = dt.DefaultView;

            kanta.Close();
        }

        private void paivitaComboBox()
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            SqlCommand komento = new SqlCommand("SELECT * FROM tuotteet", kanta);
            SqlDataReader lukija = komento.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("TUOTE", typeof(string));

            combo_tuotteet.ItemsSource = dt.DefaultView;
            combo_tuotteet.DisplayMemberPath = "TUOTE";
            combo_tuotteet.SelectedValuePath = "ID";

            while(lukija.Read())
            {
                int id = lukija.GetInt32(0);
                string tuote = lukija.GetString(1);
                dt.Rows.Add(id, tuote);
            }
            lukija.Close();
            kanta.Close();


        }

        private void painike_poista_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            string id = combo_tuotteet.SelectedValue.ToString();
            SqlCommand komento = new SqlCommand("DELETE FROM tuotteet WHERE id =" + id + ";", kanta);
            komento.ExecuteNonQuery();
            kanta.Close();

            paivitaComboBox();
            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
        }
    }
}
