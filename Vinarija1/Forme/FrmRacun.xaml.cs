using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    /// Interaction logic for FrmRacun.xaml
    /// </summary>
    public partial class FrmRacun : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();

        private bool azuriraj;
        private DataRowView pomocniRed;

        public FrmRacun()
        {
            InitializeComponent();
            dpDatum.Focus();
            konekcija = kon.KreirajKonekciju();
            PopuniPadajuceListe();
        }

        public FrmRacun(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            PopuniPadajuceListe();
            dpDatum.Focus();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
        }

        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                konekcija.Open();

                DateTime date = (DateTime)dpDatum.SelectedDate;
                string datum1 = date.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture);


                SqlCommand cmd = new SqlCommand
                {
                    Connection = konekcija
                };
            

                cmd.Parameters.Add("@datum", SqlDbType.DateTime).Value = datum1;
                cmd.Parameters.Add("@cena", SqlDbType.Float).Value = float.Parse(txtCena.Text);
                cmd.Parameters.Add("@nacinPlacanja", SqlDbType.NVarChar).Value = txtNacinPlacanja.Text;
                cmd.Parameters.Add("@vinoID", SqlDbType.Int).Value = cbxVino.SelectedValue;

                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"UPDATE Racun SET datum=@datum,
                    cena=@cena, nacinPlacanja=@nacinPlacanja, vinoID=@vinoID WHERE
                    racunID=@id";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"INSERT INTO Racun(datum, cena, nacinPlacanja, vinoID)
                            VALUES(@datum, @cena, @nacinPlacanja, @vinoID)";
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Unos određenih vrednosti nije validan.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Odaberite datum.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Cena mora biti broj!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                konekcija = kon.KreirajKonekciju();
                konekcija.Open();

                string vratiVino = @"
                        SELECT v.VinoID,
                        p.imeProizvodjaca + ', ' + 
                         a.vrstaAmbalaze + ' (' + CAST(a.mililitraza AS nvarchar) + 'ml), ' + 
                         vv.nazivVrste + ', ' + 
            CAST(v.berba AS nvarchar) + ', ' + 
           v.sorta + ', ' + 
           v.region AS prikaz
    FROM Vino v
    JOIN Proizvodjac p ON v.proizvodjacID = p.proizvodjacID
    JOIN Ambalaza a ON v.ambalazaID = a.ambalazaID
    JOIN VrstaVina vv ON v.vrstaVinaID = vv.vrstaVinaID";

                SqlDataAdapter daVino = new SqlDataAdapter(vratiVino, konekcija);
                DataTable dtVino = new DataTable();
                daVino.Fill(dtVino);
                cbxVino.ItemsSource = dtVino.DefaultView;
                daVino.Dispose();
                dtVino.Dispose();




                konekcija.Close();

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
