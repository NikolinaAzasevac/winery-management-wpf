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
    /// Interaction logic for FrmProizvodjac.xaml
    /// </summary>
    public partial class FrmProizvodjac : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();

        private bool azuriraj;
        private DataRowView pomocniRed;

        public FrmProizvodjac()
        {
            InitializeComponent();
            txtImeProizvodjaca.Focus();
            konekcija = kon.KreirajKonekciju();
        }

        public FrmProizvodjac(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            txtImeProizvodjaca.Focus();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
            konekcija = kon.KreirajKonekciju();
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
                cmd.Parameters.Add("@imeProizvodjaca", SqlDbType.NVarChar).Value = txtImeProizvodjaca.Text;
                cmd.Parameters.Add("@adresaProizvodjaca", SqlDbType.NVarChar).Value = txtAdresaProizvodjaca.Text;
                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"UPDATE Proizvodjac SET imeProizvodjaca=@imeProizvodjaca, 
                    adresaProizvodjaca=@adresaProizvodjaca WHERE
                    proizvodjacID=@id";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"INSERT INTO Proizvodjac(imeProizvodjaca, adresaProizvodjaca)
                        VALUES(@imeProizvodjaca, @adresaProizvodjaca)";
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Unos određenih vrednosti nije vallidan.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
