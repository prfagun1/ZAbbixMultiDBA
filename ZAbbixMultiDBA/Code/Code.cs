using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZAbbixMultiDBA.Model;

namespace ZAbbixMultiDBA.Code{
    class Code    {

        public static Parameter getConfiguration(String configurationFile){

            using (StreamReader r = new StreamReader(configurationFile)) {
                string json = r.ReadToEnd();

                return JsonConvert.DeserializeObject<Model.Parameter>(json);
            }

        }

        public static void sendZabbixAgent(String zabbixServer, String port, String agentPath, Boolean Debug,  String host, String trap, String message) {

            if (message == null) return;

            message = message.Replace(",",".");

            trap = "\"" + trap + "\"";

            String agentCommand = agentPath + " -z " + zabbixServer + " -p " + port + " -s \"" + host + "\" -k " + trap + " -o \"" + message + "\" -vv";
            String result = "";

            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c \"" + agentCommand + "\"");
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.CreateNoWindow = true;

            //Console.WriteLine(agentCommand);

            procStartInfo.UseShellExecute = false;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            result += agentCommand + Environment.NewLine;
            result += proc.StandardOutput.ReadToEnd();
            result += proc.StandardError.ReadToEnd();

            if(Debug) {
                Console.WriteLine(result);
            }
            else{
                if (result.Contains("failed: 1") || result.Contains("error")) {
                    Console.WriteLine("Falha ao enviar: " + result);
                }
            }

        }



        public static void xmlGenerete(Model.Parameter parameter, String configurationFile) {
            String xml = "";

            Console.Write("Informe o nome do grupo: ");
            String group = Console.ReadLine();

            Console.Write("Informe o nome da aplicação: ");
            String application = Console.ReadLine();

            Console.Write("Hosts permitidos (0.0.0.0/0): ");
            String allowedHosts = Console.ReadLine();
            if (allowedHosts == "") allowedHosts = "0.0.0.0/0";

            Console.WriteLine(group + application);

            List<String> hosts = new List<String>();


//Get host list to create XML
            foreach (Model.Select select in parameter.Zabbix.Select) {
                foreach(Model.Config config in select.Config){
                    if (!hosts.Contains(config.Host)){
                        hosts.Add(config.Host);
                    }
                }
            }


            xml += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"                                           + Environment.NewLine;
            xml += "<zabbix_export>"                                                                        + Environment.NewLine;
            xml += "    <version>3.4</version>"                                                             + Environment.NewLine;
            xml += "    <groups>"                                                                           + Environment.NewLine;
            xml += "        <group>"                                                                        + Environment.NewLine;
            xml += "            <name>" + group + "</name>"                                                 + Environment.NewLine;
            xml += "        </group>"                                                                       + Environment.NewLine;
            xml += "    </groups>"                                                                          + Environment.NewLine;
            xml += "    <hosts>"                                                                            + Environment.NewLine;
            foreach (String host in hosts) {
                xml += "        <host>"                                                                     + Environment.NewLine;
                xml += "            <host>" + host + "</host>"                                              + Environment.NewLine;
                xml += "            <description/>"                                                         + Environment.NewLine;
                xml += "            <proxy/>"                                                               + Environment.NewLine;
                xml += "            <status>0</status>"                                                     + Environment.NewLine;
                xml += "            <ipmi_authtype>-1</ipmi_authtype>"                                      + Environment.NewLine;
                xml += "            <ipmi_privilege>2</ipmi_privilege>"                                     + Environment.NewLine;
                xml += "            <ipmi_username/>"                                                       + Environment.NewLine;
                xml += "            <ipmi_password/>"                                                       + Environment.NewLine;
                xml += "            <tls_connect>1</tls_connect>"                                           + Environment.NewLine;
                xml += "            <tls_accept>1</tls_accept>"                                             + Environment.NewLine;
                xml += "            <tls_issuer/>"                                                          + Environment.NewLine;
                xml += "            <tls_subject/>"                                                         + Environment.NewLine;
                xml += "            <tls_psk_identity/>"                                                    + Environment.NewLine;
                xml += "            <tls_psk/>"                                                             + Environment.NewLine;
                xml += "            <templates/>"                                                           + Environment.NewLine;
                xml += "            <groups>"                                                               + Environment.NewLine;
                xml += "                <group>"                                                            + Environment.NewLine;
                xml += "                    <name>" + group + "</name>"                                     + Environment.NewLine;
                xml += "                </group>"                                                           + Environment.NewLine;
                xml += "            </groups>"                                                              + Environment.NewLine;
                xml += "            <interfaces>"                                                           + Environment.NewLine;
                xml += "                <interface>"                                                        + Environment.NewLine;
                xml += "                    <default>1</default>"                                           + Environment.NewLine;
                xml += "                    <type>1</type>"                                                 + Environment.NewLine;
                xml += "                    <useip>1</useip>"                                               + Environment.NewLine;
                xml += "                    <ip>127.0.0.1</ip>"                                             + Environment.NewLine;
                xml += "                    <dns/>"                                                         + Environment.NewLine;
                xml += "                    <port>10050</port>"                                             + Environment.NewLine;
                xml += "                    <bulk>1</bulk>"                                                 + Environment.NewLine;
                xml += "                    <interface_ref>if1</interface_ref>"                             + Environment.NewLine;
                xml += "                </interface>"                                                       + Environment.NewLine;
                xml += "            </interfaces>"                                                          + Environment.NewLine;
                xml += "            <applications>"                                                         + Environment.NewLine;
                xml += "                <application>"                                                      + Environment.NewLine;
                xml += "                    <name>" + application + "</name>"                               + Environment.NewLine;
                xml += "                </application>"                                                     + Environment.NewLine;
                xml += "            </applications>"                                                        + Environment.NewLine;
                xml += "            <items>"                                                                + Environment.NewLine;

                foreach (Model.Select select in parameter.Zabbix.Select) {
                    foreach(Model.Config config in select.Config){

                        xml += "                <item>" + Environment.NewLine;
                        xml += "                    <name>" + config.Trap + "</name>"                       + Environment.NewLine;
                        xml += "                    <type>2</type>"                                         + Environment.NewLine;
                        xml += "                    <snmp_community/>"                                      + Environment.NewLine;
                        xml += "                    <snmp_oid/>"                                            + Environment.NewLine;
                        xml += "                    <key>" + config.Trap + "</key>"                         + Environment.NewLine;
                        xml += "                    <delay>0</delay>"                                       + Environment.NewLine;
                        xml += "                    <history>90d</history>"                                 + Environment.NewLine;
                        xml += "                    <trends>365d</trends>"                                  + Environment.NewLine;
                        xml += "                    <status>0</status>"                                     + Environment.NewLine;
                        xml += "                    <value_type>0</value_type>"                             + Environment.NewLine;
                        xml += "                    <allowed_hosts>" + allowedHosts + "</allowed_hosts>"    + Environment.NewLine;
                        xml += "                    <units/>"                                               + Environment.NewLine;
                        xml += "                    <snmpv3_contextname/>"                                  + Environment.NewLine;
                        xml += "                    <snmpv3_securityname/>"                                 + Environment.NewLine;
                        xml += "                    <snmpv3_securitylevel>0</snmpv3_securitylevel>"         + Environment.NewLine;
                        xml += "                    <snmpv3_authprotocol>0</snmpv3_authprotocol>"           + Environment.NewLine;
                        xml += "                    <snmpv3_authpassphrase/>"                               + Environment.NewLine;
                        xml += "                    <snmpv3_privprotocol>0</snmpv3_privprotocol>"           + Environment.NewLine;
                        xml += "                    <snmpv3_privpassphrase/>"                               + Environment.NewLine;
                        xml += "                    <params/>"                                              + Environment.NewLine;
                        xml += "                    <ipmi_sensor/>"                                         + Environment.NewLine;
                        xml += "                    <authtype>0</authtype>"                                 + Environment.NewLine;
                        xml += "                    <username/>"                                            + Environment.NewLine;
                        xml += "                    <password/>"                                            + Environment.NewLine;
                        xml += "                    <publickey/>"                                           + Environment.NewLine;
                        xml += "                    <privatekey/>"                                          + Environment.NewLine;
                        xml += "                    <port/>"                                                + Environment.NewLine;
                        xml += "                    <description/>"                                         + Environment.NewLine;
                        xml += "                    <inventory_link>0</inventory_link>"                     + Environment.NewLine;
                        xml += "                    <applications>"                                         + Environment.NewLine;
                        xml += "                        <application>"                                      + Environment.NewLine;
                        xml += "                            <name>" + application + "</name>"               + Environment.NewLine;
                        xml += "                        </application>"                                     + Environment.NewLine;
                        xml += "                    </applications>"                                        + Environment.NewLine;
                        xml += "                    <valuemap/>"                                            + Environment.NewLine;
                        xml += "                    <logtimefmt/>"                                          + Environment.NewLine;
                        xml += "                    <preprocessing/>"                                       + Environment.NewLine;
                        xml += "                    <jmx_endpoint/>"                                        + Environment.NewLine;
                        xml += "                    <master_item/>"                                         + Environment.NewLine;
                        xml += "                </item>"                                                    + Environment.NewLine;
                    }
                }

            }
            xml += "            </items>"                                                                   + Environment.NewLine;
            xml += "            <discovery_rules/>"                                                         + Environment.NewLine;
            xml += "            <httptests/>"                                                               + Environment.NewLine;
            xml += "            <macros/>"                                                                  + Environment.NewLine;
            xml += "            <inventory/>"                                                               + Environment.NewLine;
            xml += "        </host>"                                                                        + Environment.NewLine;
            xml += "    </hosts>"                                                                           + Environment.NewLine;
            xml += "</zabbix_export>"                                                                       + Environment.NewLine;


            configurationFile = configurationFile.Substring(0,configurationFile.LastIndexOf("."));
            configurationFile = configurationFile + ".xml";

            System.IO.File.WriteAllText(configurationFile, xml);
        }
    }
}
