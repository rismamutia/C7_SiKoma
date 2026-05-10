using System.Data.SqlClient;

namespace CRUDMahasiswaADO
{
    public static class DatabaseHelper
    {
        public static readonly string ConnectionString =
            "Data Source=.;Initial Catalog=DBJadwalKoordinasi;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}