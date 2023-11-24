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
        string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k2101792\\Documents\\tuotekanta.mdf;Integrated Security=True;Connect Timeout=30";
        public MainWindow()
        {
            InitializeComponent();

            paivitaComboBox();
            paivitaAsiakasComboBox();

            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
            paivitaDataGrid("SELECT * FROM asiakkaat", "asiakkaat", asiakas_lista);
            paivitaDataGrid("SELECT ti.id AS id, a.nimi AS asiakas, tu.nimi AS tuote, ti.toimitettu AS toimitettu FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.toimitettu='0'", "tilaukset", tilaukset_lista);
            paivitaDataGrid("SELECT ti.id AS id, a.nimi AS asiakas, tu.nimi AS tuote, ti.toimitettu AS toimitettu FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.toimitettu='1'", "toimitetut", toimitetut_lista);

        }

        private void painike_hae_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
            }
            catch
            {
                Tilaviesti.Text = "Tietojen haku epäonnistui";
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

            Tuote_combo.ItemsSource = dt.DefaultView;
            Tuote_combo.DisplayMemberPath = "TUOTE";
            Tuote_combo.SelectedValuePath = "ID";

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

            Asiakas_combo.ItemsSource = dt.DefaultView;
            Asiakas_combo.DisplayMemberPath = "NIMI";
            Asiakas_combo.SelectedValuePath = "ID";

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
            kanta.Close();

            paivitaComboBox();
            paivitaDataGrid("SELECT * FROM tuotteet", "tuotteet", tuote_lista);
        }

        private void tuote_lista_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            int sarake = tuote_lista.CurrentCell.Column.DisplayIndex;
            solun_arvo = (e.Row.Item as DataRowView).Row[sarake].ToString();

            Tilaviesti.Text = "Sarake: " + sarake + ", Arvo: " + solun_arvo;

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

                    Tilaviesti.Text = "Uusi arvo: " + uusi_arvo;

                    paivitaComboBox();
                }
                else
                {
                    Tilaviesti.Text = "Arvo ei muuttunut";
                }

            }  
            catch 
            {
                Tilaviesti.Text = "Muokkaus ei onnistunut";
            }
        }

        private void painike_asiakas_click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection kanta = new SqlConnection(polku);
                kanta.Open();

                string sql = "INSERT INTO asiakkaat (nimi, puhelinnumero) VALUES ('" + asiakas_nimi.Text + "','" + asiakas_puhnro.Text + "')";

                SqlCommand komento = new SqlCommand(sql, kanta);
                komento.ExecuteNonQuery();

                kanta.Close();

                paivitaDataGrid("SELECT * FROM asiakkaat", "asiakkaat", asiakas_lista);

                Tilaviesti.Text = "Asiakkaan lisääminen onnistui";
            }
            catch
            {
                Tilaviesti.Text = "Muokkaus epäonnistui";
            }
        }

        private void Lisää_tilaus_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            string asiakasID = Asiakas_combo.SelectedValue.ToString();
            string tuoteID = Tuote_combo.SelectedValue.ToString();

            string sql = "INSERT INTO tilaukset (asiakas_id, tuote_id) VALUES ('"+asiakasID+"', '"+tuoteID+"')";

            SqlCommand komento = new SqlCommand(sql, kanta);
            komento.ExecuteNonQuery();

            kanta.Close();

            paivitaDataGrid("SELECT ti.id AS id, a.nimi AS asiakas, tu.nimi AS tuote, ti.toimitettu AS toimitettu FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.toimitettu='0'", "tilaukset", tilaukset_lista);

        }

        private void painike_toimita_Click(object sender, RoutedEventArgs e)
        {
            DataRowView rivinakyma = (DataRowView)((Button)e.Source).DataContext;
            String tilaus_id = rivinakyma[0].ToString();

            SqlConnection kanta = new SqlConnection(polku);
            kanta.Open();

            string sql = "UPDATE tilaukset SET toimitettu=1 WHERE id='"+tilaus_id+"';";

            SqlCommand komento = new SqlCommand(sql, kanta);
            komento.ExecuteNonQuery();

            kanta.Close();

            paivitaDataGrid("SELECT ti.id AS id, a.nimi AS asiakas, tu.nimi AS tuote, ti.toimitettu AS toimitettu FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.toimitettu='0'", "tilaukset", tilaukset_lista);
            paivitaDataGrid("SELECT ti.id AS id, a.nimi AS asiakas, tu.nimi AS tuote, ti.toimitettu AS toimitettu FROM tilaukset ti, asiakkaat a, tuotteet tu WHERE a.id=ti.asiakas_id AND tu.id=ti.tuote_id AND ti.toimitettu='1'", "toimitetut", toimitetut_lista);


        }
    }
}