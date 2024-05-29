using PBL_Project.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace PBL_Project.DAO
{
    public class CategoriaDAO : PadraoDAO<CategoriaViewModel>
    {
        protected override SqlParameter[] CriaParametros(CategoriaViewModel model)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("id", model.Id);
            parametros[1] = new SqlParameter("descricao", model.Descricao);
            return parametros;
        }

        protected override CategoriaViewModel MontaModel(DataRow registro)
        {
            CategoriaViewModel a = new CategoriaViewModel();
            a.Id = Convert.ToInt32(registro["id"]);
            a.Descricao = registro["descricao"].ToString();
            return a;
        }

        protected override void SetTabela()
        {
            Tabela = "Categoria";
        }
    }
}
