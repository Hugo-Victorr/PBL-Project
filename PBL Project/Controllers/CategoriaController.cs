using Microsoft.AspNetCore.Http;
using PBL_Project.DAO;
using PBL_Project.Models;
using System.IO;

namespace PBL_Project.Controllers
{
    public class CategoriaController : PadraoController<CategoriaViewModel>
    {
        public CategoriaController()
        {
            DAO = new CategoriaDAO();
            GeraProximoId = true;
        }

        protected override void ValidaDados(CategoriaViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            if (string.IsNullOrEmpty(model.Descricao))
                ModelState.AddModelError("Descrição", "Preencha a descrição.");

        }
    }
}
