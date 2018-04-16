using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZAbbixMultiDBA;

namespace ZAbbixMultiDBA{

    class Program    {


        static void Main(string[] args){
            Model.Parameter parameter = null;
            String configurationFile = Directory.GetCurrentDirectory() + "\\parameter.json";

            try { 
                if (args.Count() == 0) {
                    Console.WriteLine("Carregando arquivo padrão de configuração: " + configurationFile);
                }
                else {
                    configurationFile = args[0];
                }

                parameter = Code.Code.getConfiguration(configurationFile);

                try {
                    Convert.ToBoolean(parameter.Zabbix.Debug);
                }
                catch {
                    Console.WriteLine("O valor para debug somente pode ser true ou false;");
                    return;
                }
                
                
            }
            catch (Exception error) {
                Console.WriteLine("Arquivo de parametros inválido - Erro: " + error.ToString());
                return;
            }


            if (args.Count() == 2) {
                try { 
                    Code.Code.xmlGenerete(parameter, configurationFile);
                }
                catch(Exception error) {
                    Console.WriteLine("Erro ao gerar arquivo XML: " + error.ToString());
                }
                return;
            }


            List<String> logTrhead = new List<String>();

            foreach (Model.Select select in parameter.Zabbix.Select) {
                foreach (Model.Config config in select.Config) {
                    Model.Database database = parameter.Zabbix.Database.FirstOrDefault(x => x.ID == config.Database);

                    if (database == null) {
                        Console.WriteLine("Erro no select " + config.Trap  + " - Banco de dados " + config.Database + " não localizado.");
                        return;
                    }

                    String connectionString = database.ConnectionString;
                    Boolean debug = Convert.ToBoolean(parameter.Zabbix.Debug);

                    try {
                        Thread thread = new Thread(() => newThread(connectionString, select.SQL, parameter.Zabbix.Server, parameter.Zabbix.Port, parameter.Zabbix.AgentPath, debug, config.Host, config.Trap));
                        thread.Start();
                    }
                    catch(Exception error) {
                        Console.WriteLine(error.ToString());
                    }

                }

            }
        }


       private  static void newThread(String database, String sql, String zabbixServer, String port, String agentPath, Boolean debug, String host, String trap) {
            String message = Code.Database.getDatabaseValue(database, sql);
            Code.Code.sendZabbixAgent(zabbixServer, port, agentPath, debug, host, trap, message);
        }

    }

}
