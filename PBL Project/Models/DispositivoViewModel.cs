using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PBL_Project.Models
{
    public class DispositivoViewModel : PadraoViewModel
    {
        #region Atributos Json Dispositivo
        public string device_id { get; set; }
        public string entity_name { get; set; }
        public string entity_type { get; set; } = "TempSensor";
        public string protocol { get; set; } = "PDI-IoTA-UltraLight";
        public string transport { get; set; } = "MQTT";
        public List<AtributosDispositivo> attributes { get; set; }

        #endregion

        #region Propriedades Dispositivos

        [JsonIgnore]
        public string Modelo { get; set; }
        [JsonIgnore]
        public int UnidadeId { get; set; }
        [JsonIgnore]
        public DateTime DataInstalacao { get; set; }
        [JsonIgnore]
        public IFormFile Imagem { get; set; }
        [JsonIgnore]
        public byte[] ImagemEmByte { get; set; }
        [JsonIgnore]
        public string ImagemEmBase64
        {
            get
            {
                if (ImagemEmByte != null)
                    return Convert.ToBase64String(ImagemEmByte);
                else
                    return string.Empty;
            }
        }

        [JsonIgnore]
        public string UnidadeNome { get; set; }
        [JsonIgnore]
        public int EmpresaId { get; set; }
        [JsonIgnore]
        public string EmpresaNome { get; set; }
        [JsonIgnore]
        public int EstadoId { get; set; }
        [JsonIgnore]
        public string EstadoNome { get; set; }
        [JsonIgnore]
        public int CategoriaId { get; set; }
        [JsonIgnore]
        public string CategoriaNome { get; set; }
        [JsonIgnore]
        public List<double> Temperaturas { get; set; } = new List<double>();
        [JsonIgnore]
        public List<string> Tempos { get; set; } = new List<string>();
        [JsonIgnore]
        public List<double> ErroRelativo { get; set; } = new List<double>();


        #endregion

        public void PreencheAtributosDispositivo()
        {
            device_id = "TempSensor" + Id.ToString();
            entity_name = "urn:ngsi-ld:TempSensor:" + Id.ToString();
            attributes = new List<AtributosDispositivo> { new AtributosDispositivo() };
        }



        public class AtributosDispositivo
        {
            public string object_id { get; set; } = "t";
            public string name { get; set; } = "temperatura";
            public string type { get; set; } = "Integer";
        }

        public class PropriedadesGrafico
        {
            public List<string> labels { get; set; } = new List<string>();
            public List<int> dispositivos { get; set; } = new List<int>();
            public List<string> chartColors { get; set; } = new List<string>();
            public List<string> Cores { get; set; } = new List<string>
            {
            "red",
            "orange",
            "yellow",
            "green",
            "lightblue",
            "blue",
            "purple",
            "pink",
            "brown",
            "gray",
            "black"
            };
        }
    }
}
