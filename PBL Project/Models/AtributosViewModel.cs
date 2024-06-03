using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PBL_Project.Models
{

    public class AtributosViewModel : PadraoViewModel
    {
        public List<AtributoValor> attributes { get; set; }
    }

    public class AtributoValor : PadraoViewModel
    {
        public string name { get; set; }
        public List<Leitura> values { get; set; }
    }

    public class Leitura
    {
        [Newtonsoft.Json.JsonIgnore]
        public int dispositivoId { get; set; }
        public string _id { get; set; }
        public DateTime recvTime { get; set; }
        public string attrName { get; set; }
        public string attrType { get; set; }
        public int attrValue { get; set; }
    }
}
