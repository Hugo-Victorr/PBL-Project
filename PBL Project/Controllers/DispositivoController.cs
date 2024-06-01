using Microsoft.AspNetCore.Mvc.Rendering;
using PBL_Project.DAO;
using System.Collections.Generic;
using System;
using PBL_Project.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;
using System.Net.Http;
using System.Net;
using System.Text.Json.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

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
                lista.ForEach(d => d.PreencheAtributosDispositivo());

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

                ViewBag.Empresas.Insert(0, new SelectListItem("TODAS", "0"));
                ViewBag.Unidades.Insert(0, new SelectListItem("TODAS", "0"));
                ViewBag.Categorias.Insert(0, new SelectListItem("TODAS", "0"));
                ViewBag.Estados.Insert(0, new SelectListItem("TODAS", "0"));
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

        public async virtual Task<IActionResult> SaveDispositivo(DispositivoViewModel model, string Operacao)
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
                    {
                        var result = await AdicionaDispositivoFiware(model);
                        var content = await InscreveDispositivoNoSTH(model);

                        if(result && content)
                            DAO.Insert(model);
                    }
                    else
                    {
                        DAO.Update(model);
                    }

                    return RedirectToAction("IndexFiltrado", HelperDAO.unidadeId);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        //Adiciona dispositivo
        public async Task<bool> AdicionaDispositivoFiware(DispositivoViewModel model)
        {
            try
            {
                model.PreencheAtributosDispositivo();

                using (var httpClient = new HttpClient())
                {
                    string ip = "172.173.173.47";
                    string url = $"http://{ip}:4041/iot/devices";

                    var bodyObject = new
                    {
                        devices = new[]
                        {
                            new
                            {
                                device_id = model.device_id,
                                entity_name = model.entity_name,
                                entity_type = "TempSensor",
                                protocol = "PDI-IoTA-UltraLight",
                                transport = "MQTT",
                                attributes = new[]
                                {
                                    new { object_id = "t", name = "temperatura", type = "Integer" }
                                }
                            }
                        }
                    };

                    var body = JsonConvert.SerializeObject(bodyObject);

                    var content = new StringContent(body, Encoding.UTF8, "application/json");

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    requestMessage.Headers.Add("fiware-service", "smart");
                    requestMessage.Headers.Add("fiware-servicepath", "/");
                    requestMessage.Content = content;

                    using (var response = await httpClient.SendAsync(requestMessage))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string resposta = await response.Content.ReadAsStringAsync();
                            return true;
                        }
                        else
                        {
                            throw new Exception("Erro ao consultar. Code: " + response.StatusCode);
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                return false;
            }
        }


        //inscreve dispositivo no STH
        public async Task<bool> InscreveDispositivoNoSTH(DispositivoViewModel model)
        {
            try
            {
                model.PreencheAtributosDispositivo();

                using (var httpClient = new HttpClient())
                {
                    string ip = "172.173.173.47";
                    string url = $"http://{ip}:1026/v2/subscriptions";

                    var bodyObject = new
                    {
                        description = "Notify STH-Comet of all Motion Sensor count changes",
                        subject = new
                        {
                            entities = new[]
                            {
                                new { id = model.entity_name, type = "TempSensor" }
                            },
                            condition = new { attrs = new[] { "temperatura" } }
                        },
                        notification = new
                        {
                            http = new { url = url },
                            attrs = new[] { "temperatura" },
                            attrsFormat = "legacy"
                        }
                    };

                    var body = JsonConvert.SerializeObject(bodyObject);

                    var content = new StringContent(body, Encoding.UTF8, "application/json");

                    

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    requestMessage.Headers.Add("fiware-service", "smart");
                    requestMessage.Headers.Add("fiware-servicepath", "/");
                    requestMessage.Content = content;

                    using (var response = await httpClient.SendAsync(requestMessage))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string resposta = await response.Content.ReadAsStringAsync();
                            return true;
                        }
                        else
                        {
                            throw new Exception("Erro ao consultar. Code: " + response.StatusCode);
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                return false;
            }
        }

        public async Task<IActionResult> DeleteDispositivo(string device_id, int id)
        {
            try
            {
                var result = await DeletaDispositivoFiware(device_id);

                DAO.Delete(id);
                return RedirectToAction("IndexFiltrado");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public async Task<bool> DeletaDispositivoFiware(string device_id)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string ip = "172.173.173.47";
                    string url = $"http://{ip}:4041/iot/devices/{device_id}";

                    var requestMessage = new HttpRequestMessage(HttpMethod.Delete, url);

                    requestMessage.Headers.Add("fiware-service", "smart");
                    requestMessage.Headers.Add("fiware-servicepath", "/");

                    using (var response = await httpClient.SendAsync(requestMessage))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string resposta = await response.Content.ReadAsStringAsync();
                            return true;
                        }
                        else
                        {
                            throw new Exception("Erro ao consultar. Code: " + response.StatusCode);
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                return false;
            }
        }

        public bool CadastraDispositivo()
        {
            return true;
        }

        public bool EditaDispositivo()
        {
            return true;
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

                var FiwareList = ListaDispositivoFiware();

                var lista = dao.ConsultaAvancadaDispositivos(descricao, empresaId, unidadeId, categoriaId, estadoId);
                return PartialView("GridDispositivos", lista);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        public IActionResult ListaDispositivoFiware()
        {
            try
            {

                using (var httpClient = new HttpClient())
                {
                    string ip = "172.173.173.47";
                    string url = $"http://{ip}:4041/iot/devices";

                    using (var response = httpClient.GetAsync(url).Result)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            string resposta = response.Content.ReadAsStringAsync().Result;
                            return Content(resposta);
                        }
                        else
                        {
                            throw new Exception("Erro ao consultar. Code: " + response.StatusCode);
                        }
                    }
                }
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
