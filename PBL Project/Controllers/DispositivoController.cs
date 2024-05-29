using Microsoft.AspNetCore.Mvc.Rendering;
using PBL_Project.DAO;
using System.Collections.Generic;
using System;
using PBL_Project.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;

namespace PBL_Project.Controllers
{
    public class DispositivoController : PadraoController<DispositivoViewModel>
    {
        public DispositivoController()
        {
            DAO = new DispositivoDAO();
            GeraProximoId = true;
        }

        public virtual IActionResult IndexFiltrado(int id)
        {
            try
            {
                if(id != 0)
                    HelperDAO.unidadeId = id;
                DispositivoDAO dispositivoDAO = new DispositivoDAO();
                var lista = dispositivoDAO.ListagemDispositivos(HelperDAO.unidadeId);
                return View(NomeViewIndex, lista);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult ExibeConsultaAvancadaDispositivos()
        {
            try
            {
                PreparaFiltroEmpresas();
                PreparaFiltroUnidades();;
                PreparaFiltroCategorias();
                PreparaFiltroEstados();

                ViewBag.Categorias.Insert(0, new SelectListItem("TODAS", "0"));
                ViewBag.Categorias.Insert(0, new SelectListItem("TODAS", "0"));
                ViewBag.Categorias.Insert(0, new SelectListItem("TODAS", "0"));
                ViewBag.Categorias.Insert(0, new SelectListItem("TODAS", "0"));
                return View("Filtro");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.Message));
            }
        }

        public virtual IActionResult CreateDispositivo()
        {
            try
            {
                ViewBag.Operacao = "I";
                ViewBag.UnidadeId = HelperDAO.unidadeId;
                DispositivoViewModel model = new DispositivoViewModel();
                PreencheDadosParaView("I", model);
                return View(NomeViewForm, model);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public virtual IActionResult SaveDispositivo(DispositivoViewModel model, string Operacao)
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
                    return RedirectToAction("IndexFiltrado", HelperDAO.unidadeId);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult EditDispositivo(int id)
        {
            try
            {
                ViewBag.Operacao = "A";
                var model = DAO.Consulta(id);
                if (model == null)
                    return RedirectToAction(NomeViewIndex);
                else
                {
                    ViewBag.UnidadeId = HelperDAO.unidadeId;
                    PreencheDadosParaView("A", model);
                    return View(NomeViewForm, model);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        protected override void ValidaDados(DispositivoViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);

            if (string.IsNullOrEmpty(model.Modelo))
                ModelState.AddModelError("Modelo", "Preencha o modelo.");
            if (model.UnidadeId == 0)
                ModelState.AddModelError("UnidadeId", "Campo obrigatório.");
            if (model.DataInstalacao > DateTime.Now)
                ModelState.AddModelError("Data Instalacao", "Data inválida!");
            if (model.Imagem == null && operacao == "I")
                ModelState.AddModelError("Imagem", "Escolha uma imagem.");
            if (model.Imagem != null && model.Imagem.Length / 1024 / 1024 >= 2)
                ModelState.AddModelError("Imagem", "Imagem limitada a 2 mb.");
            if (ModelState.IsValid)
            {
                //na alteração, se não foi informada a imagem, iremos manter a que já estava salva.
                if (operacao == "A" && model.Imagem == null)
                {
                    DispositivoViewModel cid = DAO.Consulta(model.Id);
                    model.ImagemEmByte = cid.ImagemEmByte;
                }
                else
                {
                    model.ImagemEmByte = ConvertImageToByte(model.Imagem);
                }
            }

        }

        public IActionResult ObtemDadosConsultaAvancada(string descricao, int empresaId, int unidadeId, int categoriaId, int estadoId)
        {
            try
            {
                DispositivoDAO dao = new DispositivoDAO();
                if (string.IsNullOrEmpty(descricao))
                    descricao = "";
                if (empresaId < 1)
                    empresaId = 0;
                if (unidadeId < 1)
                    unidadeId = 0;
                if (categoriaId < 1)
                    categoriaId = 0;
                if (estadoId < 1)
                    estadoId = 0;
                var lista = dao.ConsultaAvancadaDispositivos(descricao, empresaId, unidadeId, categoriaId, estadoId);
                return PartialView("pvGridJogos", lista);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }
        protected override void PreencheDadosParaView(string Operacao, DispositivoViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
            if (Operacao == "I")
                model.DataInstalacao = DateTime.Now;
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
