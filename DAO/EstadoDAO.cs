using PBL_Project.Models;
using System.Data.SqlClient;
using System.Data;
using System.Reflection.Emit;
using System;

namespace PBL_Project.DAO
{
    public class EstadoDAO : PadraoDAO<EstadoViewModel>
    {
        protected override SqlParameter[] CriaParametros(EstadoViewModel model)
        {
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("id", model.Id);
            parametros[1] = new SqlParameter("descricao", model.Descricao);
            return parametros;
        }
        protected override EstadoViewModel MontaModel(DataRow registro)
        {
            EstadoViewModel a = new EstadoViewModel();
            a.Id = Convert.ToInt32(registro["id"]);
            a.Descricao = registro["descricao"].ToString();
            return a;
        }
        protected override void SetTabela()
        {
            Tabela = "Estado";
        }
    }
}
