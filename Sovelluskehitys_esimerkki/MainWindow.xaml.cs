using System;
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
        private string solun_arvo;
        string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k5000833\\source\\repos\\Sovelluskehitys_esimerkki\\tuotekanta.mdf;Integrated Security=True;Connect Timeout=30";
        public MainWindow()
        {
            InitializeComponent();

            paivitaComboBox();
            paivitaAsiakasComboBox();

            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
            paivitaDataGrid("SELECT * FROM asiakkaat", "asiakkaat", asiakas_lista);
            paivitaDataGrid("SELECT ti.id AS id, a.nimi AS asiakas, tu.nimi AS tuote, ti.toimitettu AS toimitettu  FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id", "tilaukset", tilaukset_lista);
        }

        private void painike_hae_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
            }
            catch 
            {
                tilaviesti.Text = "Tietojen haku epäonnistui";
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

            combo_tuotteet_2.ItemsSource = dt.DefaultView;
            combo_tuotteet_2.DisplayMemberPath = "TUOTE";
            combo_tuotteet_2.SelectedValuePath = "ID";

            while (lukija.Read()) 
            {
                int id = lukija.GetInt32(0);
                string tuote = lukija.GetString(1);
                dt.Rows.Add(id, tuote);
            }

            lukija.Close();
            kanta.Close();
        }


        private void paivitaAsiakasComboBox()
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            SqlCommand komento = new SqlCommand("SELECT * FROM asiakkaat", kanta);
            SqlDataReader lukija = komento.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("NIMI", typeof(string));

            combo_asiakkaat.ItemsSource = dt.DefaultView;
            combo_asiakkaat.DisplayMemberPath = "NIMI";
            combo_asiakkaat.SelectedValuePath = "ID";

            while (lukija.Read())
            {
                int id = lukija.GetInt32(0);
                string nimi = lukija.GetString(1);
                dt.Rows.Add(id, nimi);
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
            kanta.Close() ;

            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
            paivitaComboBox();
        }

        private void tuote_lista_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            int sarake = tuote_lista.CurrentCell.Column.DisplayIndex;
            solun_arvo = (e.Row.Item as DataRowView).Row[sarake].ToString();

            tilaviesti.Text = "Sarake: " + sarake + " Arvo: " + solun_arvo;
        }

        private void tuote_lista_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                int sarake = tuote_lista.CurrentCell.Column.DisplayIndex;
                string uusi_arvo = ((TextBox)e.EditingElement).Text;

                int tuote_id = int.Parse((e.Row.Item as DataRowView).Row[0].ToString());

                if (solun_arvo != uusi_arvo)
                {
                    string kysely = "";
                    string kanta_sarake = "";
                    SqlConnection kanta = new SqlConnection(polku);
                    kanta.Open();

                    if (sarake == 1) kanta_sarake = "nimi";
                    else if (sarake == 2) kanta_sarake = "hinta";

                    kysely = "UPDATE tuotteet SET " + kanta_sarake + "='" + uusi_arvo + "' WHERE id=" + tuote_id;

                    SqlCommand komento = new SqlCommand(kysely, kanta);
                    komento.ExecuteNonQuery();

                    kanta.Close();

                    tilaviesti.Text = "Uusi arvo: " + uusi_arvo;

                    paivitaComboBox();
                }
                else
                {
                    tilaviesti.Text = "Arvo ei muuttunut";
                }
            }
            catch
            {
                tilaviesti.Text = "Muokkaus ei onnistunut";
            }
        }

        

        private void painike_asiakas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection kanta = new SqlConnection(polku);
                kanta.Open();

                string sql = "INSERT INTO asiakkaat (nimi, puhelinnumero) VALUES ('" + asiakas_nimi.Text + "','" + asiakas_puhelin.Text + "')";

                SqlCommand komento = new SqlCommand(sql, kanta);
                komento.ExecuteNonQuery();

                kanta.Close();

                paivitaDataGrid("SELECT * FROM asiakkaat", "asiakkaat", asiakas_lista);

                tilaviesti.Text = "Asiakkaan lisääminen onnistui";
            }  
            catch
            {
                tilaviesti.Text = "Asiakkaan lisääminen ei onnistunut";
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            string asiakasID = combo_asiakkaat.SelectedValue.ToString();
            string tuoteID = combo_tuotteet_2.SelectedValue.ToString();

            string sql = "INSERT INTO tilaukset (asiakas_id, tuote_id) VALUES ('"+asiakasID+"', '"+tuoteID+"')";

            SqlCommand komento = new SqlCommand(sql, kanta);
            komento.ExecuteNonQuery();
            kanta.Close();

            paivitaDataGrid("SELECT ti.id AS id, a.nimi AS asiakas, tu.nimi AS tuote, ti.toimitettu AS toimitettu  FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id", "tilaukset", tilaukset_lista);

        }
    }
}
