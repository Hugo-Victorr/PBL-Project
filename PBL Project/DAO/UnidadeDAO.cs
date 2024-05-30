using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System;
using PBL_Project.Models;

namespace PBL_Project.DAO
{
    public class UnidadeDAO : PadraoDAO<UnidadeViewModel>
    {
        protected override SqlParameter[] CriaParametros(UnidadeViewModel model)
        {
            SqlParameter[] parametros = new SqlParameter[5];
            parametros[0] = new SqlParameter("id", model.Id);
            parametros[1] = new SqlParameter("empresaId", model.EmpresaId);
            parametros[2] = new SqlParameter("descricao", model.Descricao);
            parametros[3] = new SqlParameter("estadoId", model.EstadoId);
            parametros[4] = new SqlParameter("dataFundacao", model.DataFundacao);
            return parametros;
        }
        protected override UnidadeViewModel MontaModel(DataRow registro)
        {
            UnidadeViewModel a = new UnidadeViewModel();
            a.Id = Convert.ToInt32(registro["id"]);
            a.EmpresaId = Convert.ToInt32(registro["empresaId"]);
            a.Descricao = registro["descricao"].ToString();
            a.EstadoId = Convert.ToInt32(registro["estadoId"]);
            a.DataFundacao = Convert.ToDateTime(registro["dataFundacao"]);
            return a;
        }

        protected UnidadeViewModel MontaModelListagem(DataRow registro)
        {
            UnidadeViewModel a = new UnidadeViewModel();
            a.Id = Convert.ToInt32(registro["id"]);
            a.Descricao = registro["unidadeNome"].ToString();
            a.EmpresaId = Convert.ToInt32(registro["empresaId"]);
            a.EmpresaNome = registro["empresaNome"].ToString();
            a.EstadoId = Convert.ToInt32(registro["estadoId"]);
            a.EstadoNome = registro["estadoNome"].ToString();
            a.DataFundacao = Convert.ToDateTime(registro["dataFundacao"]);
            return a;
        }

        public virtual List<UnidadeViewModel> ListagemUnidades(int empresaId)
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("empresaId", empresaId) 
            };

            NomeSpListagem = "spListagem_UnidadesPorEmpresa";

            var tabela = HelperDAO.ExecutaProcSelect(NomeSpListagem, p);
            List<UnidadeViewModel> lista = new List<UnidadeViewModel>();

            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModelListagem(registro));

            return lista;
        }

        public List<UnidadeViewModel> ConsultaAvancadaUnidades(string descricao, int empresaId, int categoriaId, int estadoId)
        {
            SqlParameter[] p = new SqlParameter[]
            {
              new SqlParameter("descricao", descricao),
              new SqlParameter("empresaId", empresaId),
              new SqlParameter("categoriaId", categoriaId),
              new SqlParameter("estadoId", estadoId)
            };


            var tabela = HelperDAO.ExecutaProcSelect("", p);
            var lista = new List<UnidadeViewModel>();
            foreach (DataRow dr in tabela.Rows)
                lista.Add(MontaModelListagem(dr));
            return lista;
        }

        protected override void SetTabela()
        {
            Tabela = "Unidade";
        }
    }
}
