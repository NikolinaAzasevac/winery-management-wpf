using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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

namespace Vinarija1.Forme
{
    /// <summary>
    /// Interaction logic for FrmAmbalaza.xaml
    /// </summary>
    public partial class FrmAmbalaza : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();

        private bool azuriraj;
        private DataRowView pomocniRed;

        public FrmAmbalaza()
        {
            InitializeComponent();
            txtVrstaAmbalaze.Focus();
            konekcija = kon.KreirajKonekciju();
        }

        public FrmAmbalaza(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            txtVrstaAmbalaze.Focus();
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
                cmd.Parameters.Add("@vrstaAmbalaze", SqlDbType.NVarChar).Value = txtVrstaAmbalaze.Text;
                cmd.Parameters.Add("@mililitraza", SqlDbType.Int).Value = int.Parse(txtMililitraza.Text);
                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"UPDATE Ambalaza SET vrstaAmbalaze=@vrstaAmbalaze, 
                    mililitraza=@mililitraza WHERE
                    ambalazaID=@id";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"INSERT INTO Ambalaza(vrstaAmbalaze, mililitraza)
                            VALUES(@vrstaAmbalaze, @mililitraza)";
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Unos određenih vrednosti nije vallidan.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Mililitraža mora biti broj!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if(konekcija != null)
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
