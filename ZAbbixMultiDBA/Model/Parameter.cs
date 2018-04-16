using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZAbbixMultiDBA.Model{

    public class Parameter
    {
        public Zabbix Zabbix { get; set; }
    }

    public class Zabbix
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string AgentPath { get; set; }
        public string Debug { get; set; }
        public Database[] Database { get; set; }
        public Select[] Select { get; set; }
    }

    public class Database
    {
        public string ID { get; set; }
        public string ConnectionString { get; set; }
    }

    public class Select
    {
        public Config[] Config { get; set; }
        public string Description { get; set; }
        public string SQL { get; set; }
    }

    public class Config
    {
        public string Database { get; set; }
        public string Host { get; set; }
        public string Trap { get; set; }
    }



}
