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
using System.Linq;
using System.Collections;
using static PBL_Project.Models.DispositivoViewModel;

namespace PBL_Project.Controllers
{
    public class DispositivoController : PadraoController<DispositivoViewModel>
    {
        public List<Leitura> leituras = new List<Leitura>();
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

        public async Task<IActionResult> ExecutarTarefaPeriodica(int id)
         {
            try
            {
                var setPoint = 35;
                var model = DAO.Consulta(id);

                var listaLeituras = await ListaLeituras(model);

                model.Tempos.Clear();
                model.Temperaturas.Clear();
                model.ErroRelativo.Clear();

                listaLeituras.ForEach(l => model.Tempos.Add(l.recvTime.ToString("HH: mm:ss")));
                listaLeituras.ForEach(l => model.Temperaturas.Add(l.attrValue));
                listaLeituras.ForEach(l => model.ErroRelativo.Add(setPoint - l.attrValue));

                leituras = listaLeituras;

                return Json(model);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
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

                //var proxy = new WebProxy
                //{
                //    Address = new Uri("http://proxycefsa.cefsa.corp.local:8080"),
                //    BypassProxyOnLocal = true,
                //    UseDefaultCredentials = true, //não informaremos usuário e senha
                //};
                //var handler = new HttpClientHandler();
                //handler.Proxy = proxy;

                //using (var httpClient = new HttpClient(handler))
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

                //var proxy = new WebProxy
                //{
                //    Address = new Uri("http://proxycefsa.cefsa.corp.local:8080"),
                //    BypassProxyOnLocal = true,
                //    UseDefaultCredentials = true, //não informaremos usuário e senha
                //};
                //var handler = new HttpClientHandler();
                //handler.Proxy = proxy;

                //using (var httpClient = new HttpClient(handler))
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
                //var proxy = new WebProxy
                //{
                //    Address = new Uri("http://proxycefsa.cefsa.corp.local:8080"),
                //    BypassProxyOnLocal = true,
                //    UseDefaultCredentials = true, //não informaremos usuário e senha
                //};
                //var handler = new HttpClientHandler();
                //handler.Proxy = proxy;

                //using (var httpClient = new HttpClient(handler))
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

        public async Task<IActionResult> ListaDispositivosFiware()
        {
            try
            {
                //var proxy = new WebProxy
                //{
                //    Address = new Uri("http://proxycefsa.cefsa.corp.local:8080"),
                //    BypassProxyOnLocal = true,
                //    UseDefaultCredentials = true, //não informaremos usuário e senha
                //};
                //var handler = new HttpClientHandler();
                //handler.Proxy = proxy;

                //using (var httpClient = new HttpClient(handler))
                using (var httpClient = new HttpClient())
                {
                    string ip = "172.173.173.47";
                    string url = $"http://{ip}:4041/iot/devices";

                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                    requestMessage.Headers.Add("fiware-service", "smart");
                    requestMessage.Headers.Add("fiware-servicepath", "/");

                    using (var response = await httpClient.SendAsync(requestMessage))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string resposta = await response.Content.ReadAsStringAsync();

                            JObject content = JObject.Parse(resposta);

                            var dispositivos = JsonConvert.DeserializeObject<List<DispositivoViewModel>>(content.GetValue("devices").ToString());
                            return Ok(dispositivos);
                        }
                        else
                        {
                            return StatusCode((int)response.StatusCode, $"Erro ao consultar. Code: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                return StatusCode(500, $"Erro interno: {erro.Message}");
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

        public IActionResult IndexMonitoramento(int id)
        {
            try
            {
                var model = DAO.Consulta(id);

                return View("Monitoramento", model);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public async Task<List<Leitura>> ListaLeituras(DispositivoViewModel model)
        {
            try
            {
                model.PreencheAtributosDispositivo();

                //var proxy = new WebProxy
                //{
                //    Address = new Uri("http://proxycefsa.cefsa.corp.local:8080"),
                //    BypassProxyOnLocal = true,
                //    UseDefaultCredentials = true, //não informaremos usuário e senha
                //};
                //var handler = new HttpClientHandler();
                //handler.Proxy = proxy;

                //using (var httpClient = new HttpClient(handler))
                using (var httpClient = new HttpClient())
                {
                    string ip = "172.173.173.47";
                    //string url = $"http://{ip}:8666/STH/v1/contextEntities/type/TempSensor/id/{model.device_id}/attributes/temperatura?lastN=20";
                    string url = $"http://{ip}:8666/STH/v1/contextEntities/type/TempSensor/id/urn:ngsi-ld:TempSensor:01/attributes/temperatura?lastN=40";

                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                    requestMessage.Headers.Add("fiware-service", "smart");
                    requestMessage.Headers.Add("fiware-servicepath", "/");

                    using (var response = await httpClient.SendAsync(requestMessage))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string resposta = await response.Content.ReadAsStringAsync();
                            JObject jsonResponse = JObject.Parse(resposta);

                            var valuesToken = jsonResponse["contextResponses"][0]["contextElement"]["attributes"][0]["values"];
                            var valuesList = valuesToken.ToObject<List<Leitura>>();

                            return valuesList;
                        }
                        else
                        {
                            //return StatusCode((int)response.StatusCode, $"Erro ao consultar. Code: {response.StatusCode}");
                            return new List<Leitura>();
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                //return StatusCode(500, $"Erro interno: {erro.Message}");
                return new List<Leitura>();
            }
        }

        

        public IActionResult ExibeConsultaAvancadaDispositivos()
        {
            try
            {
                PreparaFiltroEmpresas();
                PreparaFiltroUnidades();
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

        public IActionResult ObtemDadosLeituras()
        {
            try
            {
                var lista = leituras.ToList();

                return PartialView("GridLeituras", lista);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        public async Task<IActionResult> ObtemDadosConsultaAvancada(string descricao, int empresaId, int unidadeId, int categoriaId, int estadoId)
        {
            try
            {
                //var FiwareList = await ListaDispositivosFiware();
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

                return PartialView("GridDispositivos", lista);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        public IActionResult ObtemDadosGraficos(string filtro)
        { 
            try
            {
                PropriedadesGrafico grafico = new PropriedadesGrafico();
                DispositivoDAO dao = new DispositivoDAO();

                var lista = dao.ListagemDispositivosInfo();

                switch (filtro)
                {
                    case "Empresa":
                        grafico = GetGraficoEmpresa(lista);
                        break;
                    case "Unidade":
                        grafico = GetGraficoUnidade(lista);
                        break;
                    case "Estado":
                        grafico = GetGraficoEstado(lista);
                        break;
                    case "Categoria":
                        grafico = GetGraficoCategoria(lista);
                        break;
                }

                return Json(grafico);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        public PropriedadesGrafico GetGraficoEmpresa(List<DispositivoViewModel> lista)
        {
            PropriedadesGrafico grafico = new PropriedadesGrafico();

            var dispositivosPorEmpresa = lista
                .GroupBy(d => new { d.EmpresaId, d.EmpresaNome })
                .Select(g => new
                {
                    EmpresaNome = g.Key.EmpresaNome,
                    QuantidadeDispositivos = g.Count()
                })
                .ToList();

            grafico.labels = dispositivosPorEmpresa.Select(e => e.EmpresaNome).ToList();
            grafico.dispositivos = dispositivosPorEmpresa.Select(e => e.QuantidadeDispositivos).ToList();

            grafico.chartColors = grafico.Cores.Take(grafico.labels.Count).ToList();

            return grafico;
        }

        public PropriedadesGrafico GetGraficoUnidade(List<DispositivoViewModel> lista)
        {
            PropriedadesGrafico grafico = new PropriedadesGrafico();

            var dispositivosPorUnidade = lista
                .GroupBy(d => new { d.UnidadeId, d.UnidadeNome })
                .Select(g => new
                {
                    UnidadeNome = g.Key.UnidadeNome,
                    QuantidadeDispositivos = g.Count()
                })
                .ToList();

            grafico.labels = dispositivosPorUnidade.Select(u => u.UnidadeNome).ToList();
            grafico.dispositivos = dispositivosPorUnidade.Select(u => u.QuantidadeDispositivos).ToList();

            grafico.chartColors = grafico.Cores.Take(grafico.labels.Count).ToList();

            return grafico;
        }

        public PropriedadesGrafico GetGraficoEstado(List<DispositivoViewModel> lista)
        {
            PropriedadesGrafico grafico = new PropriedadesGrafico();

            var dispositivosPorEstado = lista
                .GroupBy(d => new { d.EstadoId, d.EstadoNome })
                .Select(g => new
                {
                    EstadoNome = g.Key.EstadoNome,
                    QuantidadeDispositivos = g.Count()
                })
                .ToList();

            grafico.labels = dispositivosPorEstado.Select(e => e.EstadoNome).ToList();
            grafico.dispositivos = dispositivosPorEstado.Select(e => e.QuantidadeDispositivos).ToList();

            grafico.chartColors = grafico.Cores.Take(grafico.labels.Count).ToList();

            return grafico;
        }

        public PropriedadesGrafico GetGraficoCategoria(List<DispositivoViewModel> lista)
        {
            PropriedadesGrafico grafico = new PropriedadesGrafico();

            var dispositivosPorCategoria = lista
                .GroupBy(d => new { d.CategoriaId, d.CategoriaNome })
                .Select(g => new
                {
                    CategoriaNome = g.Key.CategoriaNome,
                    QuantidadeDispositivos = g.Count()
                })
                .ToList();

            grafico.labels = dispositivosPorCategoria.Select(c => c.CategoriaNome).ToList();
            grafico.dispositivos = dispositivosPorCategoria.Select(c => c.QuantidadeDispositivos).ToList();

            grafico.chartColors = grafico.Cores.Take(grafico.labels.Count).ToList();

            return grafico;
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
