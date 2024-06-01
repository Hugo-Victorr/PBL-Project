using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace PBL_Project.Models
{
    public class PadraoViewModel
    {
        [Newtonsoft.Json.JsonIgnore]
        public virtual int Id { get; set; }

    }
}
