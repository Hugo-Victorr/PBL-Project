using Microsoft.AspNetCore.Mvc.Rendering;
using PBL_Project.DAO;
using PBL_Project.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;

namespace PBL_Project.Controllers
{
    public class UnidadeController : PadraoController<UnidadeViewModel>
    {
        public UnidadeController()
        {
            DAO = new UnidadeDAO();
            GeraProximoId = true;
        }

        public IActionResult IndexFiltrado(int id)
        {
            try
            {
                if(id != 0)
                    HelperDAO.empresaId = id;

                UnidadeDAO unidadeDAO = new UnidadeDAO();
                var lista = unidadeDAO.ListagemUnidades(HelperDAO.empresaId);
                return View(NomeViewIndex, lista);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult ExibeConsultaAvancadaUnidades()
        {
            try
            {
                PreparaListaCategoriaParaCombo();
                ViewBag.Categorias.Insert(0, new SelectListItem("TODAS", "0"));
                return View("ConsultaAvancada");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.Message));
            }
        }

        public virtual IActionResult SaveUnidade(UnidadeViewModel model, string Operacao)
        {
            try
            {
                ValidaDados(model, Operacao);
                if (ModelState.IsValid == false)
                {
                    ViewBag.Operacao = Operacao;
                    PreencheDadosParaView(Operacao, model);
                    return View(NomeViewForm, model);
                }
                else
                {
                    if (Operacao == "I")
                        DAO.Insert(model);
                    else
                        DAO.Update(model);
                    return RedirectToAction("IndexFiltrado", HelperDAO.empresaId);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult EditUnidade(int id)
        {
            try
            {
                ViewBag.Operacao = "A";
                var model = DAO.Consulta(id);
                if (model == null)
                    return RedirectToAction(NomeViewIndex);
                else
                {
                    ViewBag.EmpresaId = HelperDAO.empresaId;
                    PreencheDadosParaView("A", model);
                    return View(NomeViewForm, model);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public virtual IActionResult CreateUnidade()
        {
            try
            {
                ViewBag.Operacao = "I";
                ViewBag.EmpresaId = HelperDAO.empresaId;
                UnidadeViewModel model = new UnidadeViewModel();
                PreencheDadosParaView("I", model);
                return View(NomeViewForm, model);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        protected override void ValidaDados(UnidadeViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);

            if (string.IsNullOrEmpty(model.Descricao))
                ModelState.AddModelError("Descrição", "Preencha a descrição.");
            if (model.EstadoId <= 0)
                ModelState.AddModelError("Estado", "Informe o código do estado.");
            if (model.DataFundacao > DateTime.Now)
                ModelState.AddModelError("Data de fundação", "Data inválida!");

        }

        //public IActionResult ObtemDadosConsultaAvancada(string descricao, int empresa,  int categoria, int estado)
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
        protected override void PreencheDadosParaView(string Operacao, UnidadeViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
            if (Operacao == "I")
                model.DataFundacao = DateTime.Now;

            PreparaListaEstadosParaCombo();
        }
    }
}
