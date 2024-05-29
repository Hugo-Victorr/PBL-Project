using Microsoft.AspNetCore.Http;
using PBL_Project.DAO;
using PBL_Project.Models;
using System.IO;

namespace PBL_Project.Controllers
{
    public class EstadoController : PadraoController<EstadoViewModel>
    {
        public EstadoController()
        {
            DAO = new EstadoDAO();
            GeraProximoId = true;
        }

        protected override void ValidaDados(EstadoViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            if (string.IsNullOrEmpty(model.Descricao))
                ModelState.AddModelError("Descrição", "Preencha a descrição.");
        }
    }
}
