using Microsoft.AspNetCore.Mvc.Rendering;
using PBL_Project.DAO;
using PBL_Project.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Data.SqlTypes;

namespace PBL_Project.Controllers
{
    public class EmpresaController : PadraoController<EmpresaViewModel>
    {
        public EmpresaController()
        {
            DAO = new EmpresaDAO();
            GeraProximoId = true;
        }

        public override IActionResult Index()
        {
            try
            {
                EmpresaDAO empresaDAO= new EmpresaDAO();
                var lista = empresaDAO.ListagemEmpresas();
                return View(NomeViewIndex, lista);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult ExibeConsultaAvancadaEmpresas()
        {
            try
            {
                PreparaComboCategorias();
                ViewBag.Categorias.Insert(0, new SelectListItem("TODAS", "0"));
                return View("ConsultaAvancada");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.Message));
            }
        }

        protected override void ValidaDados(EmpresaViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);

            if (string.IsNullOrEmpty(model.Descricao))
                ModelState.AddModelError("Descrição", "Preencha a descrição.");
            if (model.EstadoId <= 0)
                ModelState.AddModelError("Estado", "Informe o código do estado.");
            if (model.DataFundacao > DateTime.Now)
                ModelState.AddModelError("Data de fundação", "Data inválida!");

            if (model.Imagem == null && operacao == "I")
                ModelState.AddModelError("Imagem", "Escolha uma imagem.");
            if (model.Imagem != null && model.Imagem.Length / 1024 / 1024 >= 2)
                ModelState.AddModelError("Imagem", "Imagem limitada a 2 mb.");
            if (ModelState.IsValid)
            {
                //na alteração, se não foi informada a imagem, iremos manter a que já estava salva.
                if (operacao == "A" && model.Imagem == null)
                {
                    EmpresaViewModel cid = DAO.Consulta(model.Id);
                    model.ImagemEmByte = cid.ImagemEmByte;
                }
                else
                {
                    model.ImagemEmByte = ConvertImageToByte(model.Imagem);
                }
            }
        }


        protected override void PreencheDadosParaView(string Operacao, EmpresaViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
            if (Operacao == "I")
                model.DataFundacao = DateTime.Now;

            PreparaListaCategoriaParaCombo();
            PreparaListaEstadosParaCombo();
        }

        //public IActionResult ObtemDadosConsultaAvancada(string descricao, int categoria, int estado)
        //{
        //    try
        //    {
        //        JogoDAO dao = new JogoDAO();
        //        if (string.IsNullOrEmpty(descricao))
        //            descricao = "";
        //        if (dataInicial.Date == Convert.ToDateTime("01/01/0001"))
        //            dataInicial = SqlDateTime.MinValue.Value;
        //        if (dataFinal.Date == Convert.ToDateTime("01/01/0001"))
        //            dataFinal = SqlDateTime.MaxValue.Value;
        //        var lista = dao.ConsultaAvancadaJogos(descricao, categoria, dataInicial, dataFinal);
        //        return PartialView("pvGridJogos", lista);
        //    }
        //    catch (Exception erro)
        //    {
        //        return Json(new { erro = true, msg = erro.Message });
        //    }
        //}
        private void PreparaComboCategorias()
        {
            CategoriaDAO dao = new CategoriaDAO();
            var lista = dao.Listagem();
            List<SelectListItem> listaRetorno = new List<SelectListItem>();
            foreach (var categ in lista)
                listaRetorno.Add(new SelectListItem(categ.Descricao, categ.Id.ToString()));
            ViewBag.Categorias = listaRetorno;
        }

        public byte[] ConvertImageToByte(IFormFile file)
        {
            if (file != null)
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    return ms.ToArray();
                }
            else
                return null;
        }

    }
}
