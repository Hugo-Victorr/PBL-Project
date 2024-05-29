using System;

namespace PBL_Project.Models
{
    public class UnidadeViewModel : PadraoViewModel
    {
        public string Descricao { get; set; }
        public int EmpresaId { get; set; }
        public int EstadoId { get; set; }
        public DateTime DataFundacao { get; set; }


        public string EmpresaNome { get; set; }
        public string EstadoNome { get; set; }
    }
}
