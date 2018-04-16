using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZAbbixMultiDBA.Code{

    class Database{

        public static String getDatabaseValue(String connectionString, String sql) {
            OdbcConnection connection = null;
            OdbcDataReader reader = null;

            try{
                connection = new OdbcConnection(connectionString);
                connection.Open();
                OdbcCommand cmd = new OdbcCommand(sql, connection);

                reader = cmd.ExecuteReader();
            }
            catch(Exception error) {
                Console.WriteLine("Erro ao consultar banco: " + error.ToString());
                return null;
            }

//Se não tiver dados retorna somente o cabeçalho
            if (!reader.HasRows) {
                connection.Close();
                return null;
            }

            reader.Read();
            return reader[0].ToString();
        }

    }
}
