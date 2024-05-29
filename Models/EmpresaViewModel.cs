using Microsoft.AspNetCore.Http;
using System;

namespace PBL_Project.Models
{
    public class EmpresaViewModel : PadraoViewModel
    {
        public string Descricao { get; set; }
        public int CategoriaId { get; set; }
        public int EstadoId { get; set; }
        public DateTime DataFundacao { get; set; }

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

        public string CategoriaNome { get; set; }
        public string EstadoNome { get; set; }
    }
}
