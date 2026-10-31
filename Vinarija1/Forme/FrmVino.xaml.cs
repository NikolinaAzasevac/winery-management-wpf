using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;

namespace Vinarija1.Forme
{
    /// <summary>
    /// Interaction logic for FrmVino.xaml
    /// </summary>
    public partial class FrmVino : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();

        private bool azuriraj;
        private DataRowView pomocniRed;

        public FrmVino()
        {
            InitializeComponent();
            txtSorta.Focus();
            konekcija = kon.KreirajKonekciju();
            PopuniPadajuceListe();
        }

        public FrmVino(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            PopuniPadajuceListe();
            txtSorta.Focus();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
        }
                
        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                konekcija.Open();
                
                SqlCommand cmd = new SqlCommand
                {
                    Connection = konekcija
                };
               
                cmd.Parameters.Add("@sorta", SqlDbType.NVarChar).Value = txtSorta.Text;
                cmd.Parameters.Add("@berba", SqlDbType.Int).Value = txtBerba.Text; 
                cmd.Parameters.Add("@region", SqlDbType.NVarChar).Value = txtRegion.Text;
                cmd.Parameters.Add("@ambalazaID", SqlDbType.Int).Value = cbxAmbalaza.SelectedValue;
                cmd.Parameters.Add("@proizvodjacID", SqlDbType.Int).Value = cbxProizvodjac.SelectedValue;
                cmd.Parameters.Add("@vrstaVinaID", SqlDbType.Int).Value = cbxVrstaVina.SelectedValue;
                cmd.Parameters.Add("@kupacID", SqlDbType.Int).Value = cbxKupac.SelectedValue;

                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"UPDATE Vino SET sorta=@sorta, berba=@berba, region=@region,
                    ambalazaID=@ambalazaID, proizvodjacID=@proizvodjacID, vrstaVinaID=@vrstaVinaID, kupacID=@kupacID WHERE
                    vinoID=@id";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"INSERT INTO Vino(sorta, berba, region, ambalazaID, proizvodjacID, vrstaVinaID, kupacID)
                            VALUES(@sorta, @berba, @region, @ambalazaID, @proizvodjacID, @vrstaVinaID, @kupacID);";
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Unos određenih vrednosti nije validan.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            catch (FormatException)
            {
                MessageBox.Show("Berba mora biti ceo broj!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
        }

        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PopuniPadajuceListe()
        {
            try
            {
                konekcija.Open();

                // Ambalaza
                string vratiAmbalazu = @"SELECT ambalazaID, 
                                vrstaAmbalaze + ' (' + CAST(mililitraza AS nvarchar) + 'ml)' AS ambalaza 
                         FROM Ambalaza";
                SqlDataAdapter daAmbalaza = new SqlDataAdapter(vratiAmbalazu, konekcija);
                DataTable dtAmbalaza = new DataTable();
                daAmbalaza.Fill(dtAmbalaza);
                cbxAmbalaza.ItemsSource = dtAmbalaza.DefaultView;
                dtAmbalaza.Dispose();
                daAmbalaza.Dispose();

                // Proizvodjac
                string vratiProizvodjaca = @"SELECT proizvodjacID, imeProizvodjaca FROM Proizvodjac";
                SqlDataAdapter daProizvodjac = new SqlDataAdapter(vratiProizvodjaca, konekcija);
                DataTable dtProizvodjac = new DataTable();
                daProizvodjac.Fill(dtProizvodjac);
                cbxProizvodjac.ItemsSource = dtProizvodjac.DefaultView;
                dtProizvodjac.Dispose();
                daProizvodjac.Dispose();

                // Vrsta vina
                string vratiVrstuVina = @"SELECT vrstaVinaID, nazivVrste AS vrsta FROM VrstaVina";
                SqlDataAdapter daVrstaVina = new SqlDataAdapter(vratiVrstuVina, konekcija);
                DataTable dtVrstaVina = new DataTable();
                daVrstaVina.Fill(dtVrstaVina);
                cbxVrstaVina.ItemsSource = dtVrstaVina.DefaultView;
                dtVrstaVina.Dispose();
                daVrstaVina.Dispose();

                // Kupac
                string vratiKupca = @"SELECT kupacID, 
                             ime + ' ' + prezime + ' (' + CAST(godine AS nvarchar) + ')' AS kupac 
                      FROM Kupac";
                SqlDataAdapter daKupac = new SqlDataAdapter(vratiKupca, konekcija);
                DataTable dtKupac = new DataTable();
                daKupac.Fill(dtKupac);
                cbxKupac.ItemsSource = dtKupac.DefaultView;
                dtKupac.Dispose();
                daKupac.Dispose();



            }
            catch (SqlException)
            {
                MessageBox.Show("Padajuće liste nisu popunjene.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
        }
    }
}
