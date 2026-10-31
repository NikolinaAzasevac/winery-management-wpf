using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinarija1
{
    class Konekcija
    {
        public SqlConnection KreirajKonekciju()
        {
            SqlConnectionStringBuilder ccnSb = new SqlConnectionStringBuilder();
            ccnSb.DataSource = @"localhost\SQLEXPRESS";
            ccnSb.InitialCatalog = "vinarija1";
            ccnSb.IntegratedSecurity = true;

            ccnSb.TrustServerCertificate = true;
            ccnSb.Encrypt = false;

            string con = ccnSb.ToString();
            SqlConnection konekcija = new SqlConnection(con);
            return konekcija;
        }
        
    }
}
