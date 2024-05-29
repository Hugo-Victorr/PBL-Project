using System.Data.SqlClient;
using System.Data;
using System.Reflection.Emit;
using System;
using PBL_Project.Models;
using System.Collections.Generic;

namespace PBL_Project.DAO
{
    public class EmpresaDAO : PadraoDAO<EmpresaViewModel>
    {
        protected override SqlParameter[] CriaParametros(EmpresaViewModel model)
        {
            object imgByte = model.ImagemEmByte;
            if (imgByte == null)
                imgByte = DBNull.Value;
            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("id", model.Id);
            parametros[1] = new SqlParameter("descricao", model.Descricao);
            parametros[2] = new SqlParameter("categoriaId", model.CategoriaId);
            parametros[3] = new SqlParameter("estadoId", model.EstadoId);
            parametros[4] = new SqlParameter("dataFundacao", model.DataFundacao);
            parametros[5] = new SqlParameter("imagem", imgByte);
            return parametros;
        }
        protected override EmpresaViewModel MontaModel(DataRow registro)
        {
            EmpresaViewModel a = new EmpresaViewModel();
            a.Id = Convert.ToInt32(registro["id"]);
            a.Descricao = registro["descricao"].ToString();
            a.CategoriaId = Convert.ToInt32(registro["categoriaId"]);
            a.EstadoId = Convert.ToInt32(registro["estadoId"]);
            a.DataFundacao = Convert.ToDateTime(registro["dataFundacao"]);

            if (registro["imagem"] != DBNull.Value)
                a.ImagemEmByte = registro["imagem"] as byte[];

            return a;
        }

        protected EmpresaViewModel MontaModelListagem(DataRow registro)
        {
            EmpresaViewModel a = new EmpresaViewModel();
            a.Id = Convert.ToInt32(registro["id"]);
            a.Descricao = registro["descricao"].ToString();
            a.CategoriaId = Convert.ToInt32(registro["categoriaId"]);
            a.CategoriaNome = registro["categoriaNome"].ToString();
            a.EstadoId = Convert.ToInt32(registro["estadoId"]);
            a.EstadoNome = registro["estadoNome"].ToString();
            a.DataFundacao = Convert.ToDateTime(registro["dataFundacao"]);

            if (registro["imagem"] != DBNull.Value)
                a.ImagemEmByte = registro["imagem"] as byte[];

            return a;
        }

        public virtual List<EmpresaViewModel> FiltraEmpresas(int estadoId, int categoriaId)
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("estadoId", estadoId), 
                new SqlParameter("categoriaId", categoriaId) 
            };

            NomeSpListagem = "spListagem_EmpresasPorEstadoCategoria";

            var tabela = HelperDAO.ExecutaProcSelect(NomeSpListagem, p);
            List<EmpresaViewModel> lista = new List<EmpresaViewModel>();

            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModelListagem(registro));

            return lista;
        }

        public virtual List<EmpresaViewModel> ListagemEmpresas()
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("tabela", Tabela),
                new SqlParameter("Ordem", "1") // 1 é o primeiro campo da tabela
            };

            var tabela = HelperDAO.ExecutaProcSelect(NomeSpListagem, p);
            List<EmpresaViewModel> lista = new List<EmpresaViewModel>();

            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModelListagem(registro));

            return lista;
        }

        protected override void SetTabela()
        {
            Tabela = "Empresa";
            NomeSpListagem = "spListagem_Empresas";
        }

    }
}
