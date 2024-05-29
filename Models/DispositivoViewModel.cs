using Microsoft.AspNetCore.Http;
using System;

namespace PBL_Project.Models
{
    public class DispositivoViewModel : PadraoViewModel
    {
        public string Modelo { get; set; }
        public int UnidadeId { get; set; }
        public DateTime DataInstalacao { get; set; }
        public IFormFile Imagem { get; set; }
        public byte[] ImagemEmByte { get; set; }
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

        public string UnidadeNome { get; set; }
    }
}
