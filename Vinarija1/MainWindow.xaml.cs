using System;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using Vinarija1.Forme;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vinarija1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Select upiti
        string ambalazaSelect = @"SELECT ambalazaID AS 'ID', vrstaAmbalaze AS 'Vrsta ambalaže', CAST(mililitraza AS NVARCHAR) AS 'Mililitraža (ml)' FROM Ambalaza";
        string kupacSelect = @"SELECT kupacID AS 'ID', ime AS 'Ime', prezime AS 'Prezime', godine AS 'Godine', nacinPlacanja AS 'Način plaćanja' FROM Kupac";
        string proizvodjacSelect = @"SELECT proizvodjacID AS 'ID', imeProizvodjaca AS 'Ime proizvođača', adresaProizvodjaca AS 'Adresa proizvođača' FROM Proizvodjac";
        string verifikatorSelect = @"SELECT verifikatorID AS 'ID', ime AS 'Ime', prezime AS 'Prezime', licenca AS 'Licenca' FROM Verifikator";
        string vrstaVinaSelect = @"SELECT vrstaVinaID AS 'ID', nazivVrste AS 'Vrsta vina' FROM VrstaVina";
        string zaposleniSelect = @"SELECT zaposleniID AS 'ID', ime AS 'Ime', prezime AS 'Prezime', jmbg AS 'JMBG', adresa AS 'Adresa', grad AS 'Grad', kontakt AS 'Kontakt' FROM Zaposleni";
        string vinoSelect = @"SELECT vinoID AS 'ID', sorta AS 'Sorta', berba AS 'Berba', region AS 'Region', 
                                Ambalaza.vrstaAmbalaze + ', ' + CAST(Ambalaza.mililitraza AS nvarchar) + 'ml' AS 'Ambalaža', 
                                Proizvodjac.imeProizvodjaca AS 'Ime proizvođača',
                                VrstaVina.nazivVrste AS 'Vrsta', 
                                Kupac.ime + ' ' + Kupac.prezime AS 'Ime i prezime kupca'
                                FROM Vino 
                                JOIN Ambalaza ON Vino.ambalazaID = Ambalaza.ambalazaID
                                JOIN Proizvodjac ON Vino.proizvodjacID = Proizvodjac.proizvodjacID
                                JOIN VrstaVina ON Vino.vrstaVinaID = VrstaVina.vrstaVinaID
                                JOIN Kupac ON Vino.kupacID = Kupac.kupacID";
        string racunSelect = @"SELECT racunID AS 'ID', datum AS 'Datum', cena AS 'Cena', nacinPlacanja AS 'Način plaćanja',
                                sorta + ', ' + CAST(berba AS NVARCHAR) + ', ' + region AS 'Vino'
                                FROM Racun
                                JOIN Vino ON Racun.vinoID = Vino.vinoID";
        string potvrdaOAutenticnostiSelect = @"SELECT
                                                potvrdaID AS 'ID',datumIzdavanjaPotvrde AS 'Datum izdavanja',
                                                Proizvodjac.imeProizvodjaca + ', ' + 
                                                Ambalaza.vrstaAmbalaze + ', ' + 
                                                CAST(Ambalaza.mililitraza AS nvarchar) + 'ml, ' + 
                                                VrstaVina.nazivVrste + ', ' + 
                                                Vino.sorta + ', ' + 
                                                Vino.region AS 'Vino',
                                                Verifikator.ime + ' ' + Verifikator.prezime + ' (' + Verifikator.licenca + ')' AS 'Verifikator'
                                            FROM PotvrdaOAutenticnosti
                                            JOIN Vino ON PotvrdaOAutenticnosti.vinoID = Vino.vinoID
                                            JOIN Proizvodjac ON Vino.proizvodjacID = Proizvodjac.proizvodjacID
                                            JOIN Ambalaza ON Vino.ambalazaID = Ambalaza.ambalazaID
                                            JOIN VrstaVina ON Vino.vrstaVinaID = VrstaVina.vrstaVinaID
                                            JOIN Verifikator ON PotvrdaOAutenticnosti.verifikatorID = Verifikator.verifikatorID";
        #endregion
        #region Delete upiti
        string proizvodjacDelete = @"DELETE FROM Proizvodjac WHERE proizvodjacID=";
        string ambalazaDelete = @"DELETE FROM Ambalaza WHERE ambalazaID=";
        string vrstaVinaDelete = @"DELETE FROM VrstaVina WHERE vrstaVinaID=";
        string kupacDelete = @"DELETE FROM Kupac WHERE kupacID=";
        string zaposleniDelete = @"DELETE FROM Zaposleni WHERE zaposleniID=";
        string verifikatorDelete = @"DELETE FROM Verifikator WHERE verifikatorID=";
        string vinoDelete = @"DELETE FROM Vino WHERE vinoID=";
        string racunDelete = @"DELETE FROM Racun WHERE racunID=";
        string potvrdaOAutenticnostiDelete = @"DELETE FROM PotvrdaOAutenticnosti WHERE potvrdaID=";

        #endregion
        #region Select upiti sa uslovima

        string selectUslovAmbalaza = @"SELECT * FROM Ambalaza WHERE ambalazaID=";
        string selectUslovProizvodjac = @"SELECT * FROM Proizvodjac WHERE proizvodjacID=";
        string selectUslovVrstaVina = @"SELECT * FROM VrstaVina WHERE vrstaVinaID=";
        string selectUslovKupac = @"SELECT * FROM Kupac WHERE kupacID=";
        string selectUslovVino = @"SELECT * FROM Vino WHERE vinoID=";
        string selectUslovVerifikator = @"SELECT * FROM Verifikator WHERE verifikatorID=";
        string selectUslovPotvrdaOAutenticnosti = @"SELECT * FROM PotvrdaOAutenticnosti WHERE potvrdaID=";
        string selectUslovZaposleni = @"SELECT * FROM Zaposleni WHERE zaposleniID=";
        string selectUslovRacun = @"SELECT * FROM Racun WHERE racunID=";

        #endregion
        Konekcija kon = new Konekcija();
        SqlConnection konekcija = new SqlConnection();
        bool azuriraj;
        DataRowView pomocniRed;
        string ucitanaTabela;
        public MainWindow()
        {
            InitializeComponent();
            UcitajPodatke(dataGridCentralni, proizvodjacSelect);
            konekcija = kon.KreirajKonekciju();

        }

        void UcitajPodatke(DataGrid grid, string selectUpiti)
        {
            try
            {
                konekcija = kon.KreirajKonekciju();
                konekcija.Open();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(selectUpiti, konekcija);
                DataTable dt = new DataTable
                {
                    Locale = CultureInfo.InvariantCulture
                };

                dataAdapter.Fill(dt);
                if (grid != null)
                {
                    grid.ItemsSource = dt.DefaultView;
                }
                ucitanaTabela = selectUpiti;
                dt.Dispose();
                dataAdapter.Dispose();


            }
            catch (SqlException ex)
            {
                MessageBox.Show("Neuspešno učitani podaci" + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
        }
        private void btnProizvodjac_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, proizvodjacSelect);
        }

        private void btnAmbalaza_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, ambalazaSelect);
        }

        private void btnVrstaVina_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, vrstaVinaSelect);
        }

        private void btnKupac_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, kupacSelect);
        }

        private void btnZaposleni_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, zaposleniSelect);
        }

        private void btnVerifikator_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, verifikatorSelect);
        }

        private void btnVino_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, vinoSelect);
        }

        private void btnPotvrdaOAutenticnosti_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, potvrdaOAutenticnostiSelect);
        }

        private void btnRacun_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, racunSelect);
        }

        private void btnDodaj_Click(object sender, RoutedEventArgs e)
        {
            Window prozor;
            if (ucitanaTabela.Equals(proizvodjacSelect))
            {
                prozor = new FrmProizvodjac();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, proizvodjacSelect);
            }
            else if (ucitanaTabela.Equals(ambalazaSelect))
            {
                prozor = new FrmAmbalaza();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, ambalazaSelect);
            }
            else if (ucitanaTabela.Equals(kupacSelect))
            {
                prozor = new FrmKupac();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, kupacSelect);
            }
            else if (ucitanaTabela.Equals(zaposleniSelect))
            {
                prozor = new FrmZaposleni();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, zaposleniSelect);
            }
            else if (ucitanaTabela.Equals(vrstaVinaSelect))
            {
                prozor = new FrmVrstaVina();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, vrstaVinaSelect);
            }
            else if (ucitanaTabela.Equals(verifikatorSelect))
            {
                prozor = new FrmVerifikator();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, verifikatorSelect);
            }
            else if (ucitanaTabela.Equals(vinoSelect))
            {
                prozor = new FrmVino();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, vinoSelect);
            }
            else if (ucitanaTabela.Equals(potvrdaOAutenticnostiSelect))
            {
                prozor = new FrmPotvrda();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, potvrdaOAutenticnostiSelect);
            }
            else if (ucitanaTabela.Equals(racunSelect))
            {
                prozor = new FrmRacun();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, racunSelect);
            }


        }

        private void btnObrisi_Click(object sender, RoutedEventArgs e)
        {
            if (ucitanaTabela.Equals(proizvodjacSelect))
            {
                ObrisiZapis(dataGridCentralni, proizvodjacDelete);
                UcitajPodatke(dataGridCentralni, proizvodjacSelect);
            }
            else if (ucitanaTabela.Equals(ambalazaSelect))
            {
                ObrisiZapis(dataGridCentralni, ambalazaDelete);
                UcitajPodatke(dataGridCentralni, ambalazaSelect);
            }
            else if (ucitanaTabela.Equals(vrstaVinaSelect))
            {
                ObrisiZapis(dataGridCentralni, vrstaVinaDelete);
                UcitajPodatke(dataGridCentralni, vrstaVinaSelect);
            }
            else if (ucitanaTabela.Equals(kupacSelect))
            {
                ObrisiZapis(dataGridCentralni, kupacDelete);
                UcitajPodatke(dataGridCentralni, kupacSelect);
            }
            else if (ucitanaTabela.Equals(zaposleniSelect))
            {
                ObrisiZapis(dataGridCentralni, zaposleniDelete);
                UcitajPodatke(dataGridCentralni, zaposleniSelect);
            }
            else if (ucitanaTabela.Equals(verifikatorSelect))
            {
                ObrisiZapis(dataGridCentralni, verifikatorDelete);
                UcitajPodatke(dataGridCentralni, verifikatorSelect);
            }
            else if (ucitanaTabela.Equals(vinoSelect))
            {
                ObrisiZapis(dataGridCentralni, vinoDelete);
                UcitajPodatke(dataGridCentralni, vinoSelect);
            }
            else if (ucitanaTabela.Equals(racunSelect))
            {
                ObrisiZapis(dataGridCentralni, racunDelete);
                UcitajPodatke(dataGridCentralni, racunSelect);
            }
            else if (ucitanaTabela.Equals(potvrdaOAutenticnostiSelect))
            {
                ObrisiZapis(dataGridCentralni, potvrdaOAutenticnostiDelete);
                UcitajPodatke(dataGridCentralni, potvrdaOAutenticnostiSelect);
            }

        }

        private void ObrisiZapis(DataGrid grid, object deleteUpit)
        {
            try
            {
                konekcija.Open();
                DataRowView red = (DataRowView)grid.SelectedItems[0];
                MessageBoxResult rezultat = MessageBox.Show("Da li ste sigurni?", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (rezultat == MessageBoxResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand
                    {
                        Connection = konekcija
                    };
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = deleteUpit + "@id";
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Niste selektovali red.", "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (SqlException)
            {
                MessageBox.Show("Postoje povezani podaci u drugim tabelama", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
        }

        private void PopuniFormu(DataGrid grid, string selectUslov)
        {
            try
            {
                konekcija.Open();
                azuriraj = true;
                DataRowView red = (DataRowView)grid.SelectedItems[0];
                pomocniRed = red;
                SqlCommand cmd = new SqlCommand
                {
                    Connection = konekcija,
                };
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                cmd.CommandText = selectUslov + "@id";
                SqlDataReader citac = cmd.ExecuteReader();
                cmd.Dispose();
                while (citac.Read())
                {
                    if (ucitanaTabela.Equals(ambalazaSelect, StringComparison.Ordinal))
                    {
                        FrmAmbalaza prozor = new FrmAmbalaza(azuriraj, pomocniRed);
                        prozor.txtVrstaAmbalaze.Text = citac["vrstaAmbalaze"].ToString();
                        prozor.txtMililitraza.Text = citac["mililitraza"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(proizvodjacSelect, StringComparison.Ordinal))
                    {
                        FrmProizvodjac prozor = new FrmProizvodjac(azuriraj, pomocniRed);
                        prozor.txtImeProizvodjaca.Text = citac["imeProizvodjaca"].ToString();
                        prozor.txtAdresaProizvodjaca.Text = citac["adresaProizvodjaca"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(vrstaVinaSelect, StringComparison.Ordinal))
                    {
                        FrmVrstaVina prozor = new FrmVrstaVina(azuriraj, pomocniRed);
                        prozor.txtNazivVrste.Text = citac["nazivVrste"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(kupacSelect, StringComparison.Ordinal))
                    {
                        FrmKupac prozor = new FrmKupac(azuriraj, pomocniRed);
                        prozor.txtIme.Text = citac["ime"].ToString();
                        prozor.txtPrezime.Text = citac["prezime"].ToString();
                        prozor.txtGodine.Text = citac["godine"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(vinoSelect, StringComparison.Ordinal))
                    {
                        FrmVino prozor = new FrmVino(azuriraj, pomocniRed);
                        prozor.txtSorta.Text = citac["sorta"].ToString();
                        prozor.txtBerba.Text = citac["berba"].ToString();
                        prozor.txtRegion.Text = citac["region"].ToString();
                        prozor.cbxAmbalaza.SelectedValue = citac["ambalazaID"].ToString();
                        prozor.cbxProizvodjac.SelectedValue = citac["proizvodjacID"].ToString();
                        prozor.cbxVrstaVina.SelectedValue = citac["vrstaVinaID"].ToString();
                        prozor.cbxKupac.SelectedValue = citac["kupacID"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(verifikatorSelect, StringComparison.Ordinal))
                    {
                        FrmVerifikator prozor = new FrmVerifikator(azuriraj, pomocniRed);
                        prozor.txtIme.Text = citac["ime"].ToString();
                        prozor.txtPrezime.Text = citac["prezime"].ToString();
                        prozor.txtLicenca.Text = citac["licenca"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(potvrdaOAutenticnostiSelect, StringComparison.Ordinal))
                    {
                        FrmPotvrda prozor = new FrmPotvrda(azuriraj, pomocniRed);
                        //prozor.dpDatum.SelectedDate = (DateTime)["datumIzdavanjaPotvrde"];
                        prozor.dpDatum.SelectedDate = DateTime.Parse(citac["datumIzdavanjaPotvrde"].ToString());
                        prozor.cbxVino.SelectedValue = citac["vinoID"].ToString();
                        prozor.cbxVerifikator.SelectedValue = citac["verifikatorID"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(zaposleniSelect, StringComparison.Ordinal))
                    {
                        FrmZaposleni prozor = new FrmZaposleni(azuriraj, pomocniRed);
                        prozor.txtIme.Text = citac["ime"].ToString();
                        prozor.txtPrezime.Text = citac["prezime"].ToString();
                        prozor.txtJmbg.Text = citac["jmbg"].ToString();
                        prozor.txtAdresa.Text = citac["adresa"].ToString();
                        prozor.txtGrad.Text = citac["grad"].ToString();
                        prozor.txtKontakt.Text = citac["kontakt"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(racunSelect, StringComparison.Ordinal))
                    {
                        FrmRacun prozor = new FrmRacun(azuriraj, pomocniRed);
                        prozor.dpDatum.SelectedDate = DateTime.Parse(citac["datum"].ToString());
                        prozor.cbxVino.SelectedValue = citac["vinoID"].ToString();
                        prozor.txtCena.Text = citac["cena"].ToString();
                        prozor.txtNacinPlacanja.Text = citac["nacinPlacanja"].ToString();
                        prozor.ShowDialog();
                    }

                }
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Niste selektovali red.", "Greška!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
                azuriraj = false;
            }
        }

        private void btnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            if (ucitanaTabela.Equals(ambalazaSelect))
            {
                PopuniFormu(dataGridCentralni, selectUslovAmbalaza);
                UcitajPodatke(dataGridCentralni, ambalazaSelect);
            }

            else if (ucitanaTabela.Equals(proizvodjacSelect))
            {
                PopuniFormu(dataGridCentralni, selectUslovProizvodjac);
                UcitajPodatke(dataGridCentralni, proizvodjacSelect);
            }
            else if (ucitanaTabela.Equals(vrstaVinaSelect))
            {
                PopuniFormu(dataGridCentralni, selectUslovVrstaVina);
                UcitajPodatke(dataGridCentralni, vrstaVinaSelect);
            }
            else if (ucitanaTabela.Equals(kupacSelect))
            {
                PopuniFormu(dataGridCentralni, selectUslovKupac);
                UcitajPodatke(dataGridCentralni, kupacSelect);
            }
            else if (ucitanaTabela.Equals(vinoSelect))
            {
                PopuniFormu(dataGridCentralni, selectUslovVino);
                UcitajPodatke(dataGridCentralni, vinoSelect);
            }
            else if (ucitanaTabela.Equals(verifikatorSelect))
            {
                PopuniFormu(dataGridCentralni, selectUslovVerifikator);
                UcitajPodatke(dataGridCentralni, verifikatorSelect);
            }
            else if (ucitanaTabela.Equals(potvrdaOAutenticnostiSelect))
            {
                PopuniFormu(dataGridCentralni, selectUslovPotvrdaOAutenticnosti);
                UcitajPodatke(dataGridCentralni, potvrdaOAutenticnostiSelect);
            }
            else if (ucitanaTabela.Equals(zaposleniSelect))
            {
                PopuniFormu(dataGridCentralni, selectUslovZaposleni);
                UcitajPodatke(dataGridCentralni, zaposleniSelect);
            }
            else if (ucitanaTabela.Equals(racunSelect))
            {
                PopuniFormu(dataGridCentralni, selectUslovRacun);
                UcitajPodatke(dataGridCentralni, racunSelect);
            }
        }
    }
}