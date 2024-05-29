using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PBL_Project.DAO;
using PBL_Project.Models;
using System;
using System.Collections.Generic;

namespace PBL_Project.Controllers
{
    public class PadraoController<T> : Controller where T : PadraoViewModel
    {
        protected PadraoDAO<T> DAO { get; set; }
        protected bool GeraProximoId { get; set; }
        protected string NomeViewIndex { get; set; } = "Index";
        protected string NomeViewForm { get; set; } = "Form";
        protected string NomeActionIndex { get; set; } = "IndexFiltrado";

        public virtual IActionResult Index()
        {
            try
            {
                var lista = DAO.Listagem();
                return View(NomeViewIndex, lista);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        public virtual IActionResult Create()
        {
            try
            {
                ViewBag.Operacao = "I";
                T model = Activator.CreateInstance(typeof(T)) as T;
                PreencheDadosParaView("I", model);
                return View(NomeViewForm, model);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        protected virtual void PreencheDadosParaView(string Operacao, T model)
        {
            if (GeraProximoId && Operacao == "I")
                model.Id = DAO.ProximoId();
        }

        public void PreparaListaEmpresasParaCombo()
        {
            EmpresaDAO empresaDAO = new EmpresaDAO();
            var empresas = empresaDAO.Listagem();
            List<SelectListItem> listaEmpresas = new List<SelectListItem>();
            listaEmpresas.Add(new SelectListItem("Selecione uma empresa...", "-1"));
            foreach (var empresa in empresas)
            {
                SelectListItem item = new SelectListItem(empresa.Descricao, empresa.Id.ToString());
                listaEmpresas.Add(item);
            }
            ViewBag.Empresas = listaEmpresas;
        }

        public void PreparaListaUnidadesParaCombo()
        {
            UnidadeDAO unidadeDAO = new UnidadeDAO();
            var unidades = unidadeDAO.Listagem();
            List<SelectListItem> listaUnidades = new List<SelectListItem>();
            listaUnidades.Add(new SelectListItem("Selecione uma unidade...", "-1"));
            foreach (var empresa in unidades)
            {
                SelectListItem item = new SelectListItem(empresa.Descricao, empresa.Id.ToString());
                listaUnidades.Add(item);
            }
            ViewBag.Empresas = listaUnidades;
        }
        public void PreparaListaCategoriasParaCombo()
        {
            CategoriaDAO categoriaDao = new CategoriaDAO();
            var categorias = categoriaDao.Listagem();
            List<SelectListItem> listaCategorias = new List<SelectListItem>();
            listaCategorias.Add(new SelectListItem("Selecione uma categoria...", "-1"));
            foreach (var categoria in categorias)
            {
                SelectListItem item = new SelectListItem(categoria.Descricao, categoria.Id.ToString());
                listaCategorias.Add(item);
            }
            ViewBag.Categorias = listaCategorias;
        }

        protected void PreparaListaEstadosParaCombo()
        {
            EstadoDAO estadoDao = new EstadoDAO();
            var estados = estadoDao.Listagem();
            List<SelectListItem> listaEstados = new List<SelectListItem>();
            listaEstados.Add(new SelectListItem("Selecione um estado...", "-1"));
            foreach (var estado in estados)
            {
                SelectListItem item = new SelectListItem(estado.Descricao, estado.Id.ToString());
                listaEstados.Add(item);
            }
            ViewBag.Estados = listaEstados;
        }

        public void PreparaFiltroCategorias()
        {
            CategoriaDAO dao = new CategoriaDAO();
            var lista = dao.Listagem();
            List<SelectListItem> listaRetorno = new List<SelectListItem>();
            foreach (var categ in lista)
                listaRetorno.Add(new SelectListItem(categ.Descricao, categ.Id.ToString()));
            ViewBag.Categorias = listaRetorno;
        }

        public void PreparaFiltroEstados()
        {
            EstadoDAO dao = new EstadoDAO();
            var lista = dao.Listagem();
            List<SelectListItem> listaRetorno = new List<SelectListItem>();
            foreach (var estado in lista)
                listaRetorno.Add(new SelectListItem(estado.Descricao, estado.Id.ToString()));
            ViewBag.Estados = listaRetorno;
        }

        public void PreparaFiltroEmpresas()
        {
            EmpresaDAO dao = new EmpresaDAO();
            var lista = dao.Listagem();
            List<SelectListItem> listaRetorno = new List<SelectListItem>();
            foreach (var empresa in lista)
                listaRetorno.Add(new SelectListItem(empresa.Descricao, empresa.Id.ToString()));
            ViewBag.Empresas = listaRetorno;
        }
        public void PreparaFiltroUnidades()
        {
            UnidadeDAO dao = new UnidadeDAO();
            var lista = dao.Listagem();
            List<SelectListItem> listaRetorno = new List<SelectListItem>();
            foreach (var unidade in lista)
                listaRetorno.Add(new SelectListItem(unidade.Descricao, unidade.Id.ToString()));
            ViewBag.Unidades = listaRetorno;
        }

        public virtual IActionResult Save(T model, string Operacao)
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
                    return RedirectToAction(NomeViewIndex);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        protected virtual void ValidaDados(T model, string operacao)
        {
            ModelState.Clear();
            if (operacao == "I" && DAO.Consulta(model.Id) != null)
                ModelState.AddModelError("Id", "Código já está em uso!");
            if (operacao == "A" && DAO.Consulta(model.Id) == null)
                ModelState.AddModelError("Id", "Este registro não existe!");
            if (model.Id <= 0)
                ModelState.AddModelError("Id", "Id inválido!");
        }
        public IActionResult Edit(int id)
        {
            try
            {
                ViewBag.Operacao = "A";
                var model = DAO.Consulta(id);
                if (model == null)
                    return RedirectToAction(NomeViewIndex);
                else
                {
                    PreencheDadosParaView("A", model);
                    return View(NomeViewForm, model);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                DAO.Delete(id);
                return RedirectToAction(NomeViewIndex);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
    }

}
