using PBL_Project.Models;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;

namespace PBL_Project.DAO
{
    public class DispositivoDAO : PadraoDAO<DispositivoViewModel>
    {
        protected override SqlParameter[] CriaParametros(DispositivoViewModel model)
        {
            object imgByte = model.ImagemEmByte;
            if (imgByte == null)
                imgByte = DBNull.Value;
            SqlParameter[] parametros = new SqlParameter[5];
            parametros[0] = new SqlParameter("id", model.Id);
            parametros[1] = new SqlParameter("modelo", model.Modelo);
            parametros[2] = new SqlParameter("unidadeId", model.UnidadeId);
            parametros[3] = new SqlParameter("dataInstalacao", model.DataInstalacao);
            parametros[4] = new SqlParameter("imagem", imgByte);
            return parametros;
        }

        protected override DispositivoViewModel MontaModel(DataRow registro)
        {
            DispositivoViewModel a = new DispositivoViewModel();
            a.Id = Convert.ToInt32(registro["id"]);
            a.Modelo = registro["modelo"].ToString();
            a.UnidadeId = Convert.ToInt32(registro["unidadeId"]);
            a.DataInstalacao = Convert.ToDateTime(registro["dataInstalacao"]);

            if (registro["imagem"] != DBNull.Value)
                a.ImagemEmByte = registro["imagem"] as byte[];

            return a;
        }

        protected DispositivoViewModel MontaModelListagem(DataRow registro)
        {
            DispositivoViewModel a = new DispositivoViewModel();
            a.Id = Convert.ToInt32(registro["id"]);
            a.Modelo = registro["modelo"].ToString();
            a.DataInstalacao = Convert.ToDateTime(registro["dataInstalacao"]);
            a.UnidadeId = Convert.ToInt32(registro["unidadeId"]);
            a.UnidadeNome = registro["unidadeNome"].ToString();

            if (registro["imagem"] != DBNull.Value)
                a.ImagemEmByte = registro["imagem"] as byte[];

            return a;
        }

        protected DispositivoViewModel MontaModelListagemAvancada(DataRow registro)
        {
            DispositivoViewModel a = new DispositivoViewModel();
            a.Id = Convert.ToInt32(registro["id"]);
            a.Modelo = registro["modelo"].ToString();
            a.DataInstalacao = Convert.ToDateTime(registro["dataInstalacao"]);
            a.UnidadeId = Convert.ToInt32(registro["unidadeId"]);
            a.UnidadeNome = registro["unidadeNome"].ToString();
            a.EmpresaId = Convert.ToInt32(registro["empresaId"]);
            a.EstadoId = Convert.ToInt32(registro["estadoId"]);
            a.CategoriaId = Convert.ToInt32(registro["categoriaId"]);
            a.EmpresaNome = registro["empresaNome"].ToString();
            a.EstadoNome = registro["estadoNome"].ToString();
            a.CategoriaNome = registro["categoriaNome"].ToString();

            if (registro["imagem"] != DBNull.Value)
                a.ImagemEmByte = registro["imagem"] as byte[];

            return a;
        }

        public virtual List<DispositivoViewModel> ListagemDispositivos(int unidadeId)
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("unidadeId", unidadeId) 
            };

            NomeSpListagem = "spListagem_DispositivoPorUnidade";

            var tabela = HelperDAO.ExecutaProcSelect(NomeSpListagem, p);
            List<DispositivoViewModel> lista = new List<DispositivoViewModel>();

            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModelListagem(registro));

            return lista;
        }

        public List<DispositivoViewModel> ConsultaAvancadaDispositivos(string descricao, int empresaId, int unidadeId, int categoriaId, int estadoId)
        {
            SqlParameter[] p = new SqlParameter[]
            {
              new SqlParameter("descricao", descricao),
              new SqlParameter("empresaId", empresaId),
              new SqlParameter("unidadeId", unidadeId),
              new SqlParameter("categoriaId", categoriaId),
              new SqlParameter("estadoId", estadoId)
            };

            var tabela = HelperDAO.ExecutaProcSelect("spListagemAvancada_Dispositivos", p);
            var lista = new List<DispositivoViewModel>();
            foreach (DataRow dr in tabela.Rows)
                lista.Add(MontaModelListagemAvancada(dr));
            return lista;
        }

        protected override void SetTabela()
        {
            Tabela = "Dispositivo";
        }
    }
}
