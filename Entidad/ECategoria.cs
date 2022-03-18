using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class ECategoria
    {
        public long IdCategoria { get; set; }
        public long id { get; set; }
        public string Nombre { get; set; }
        public string text { get; set; }
        public long IdUsuario { get; set; }
        public long IdEmpresa { get; set; }
        public Nullable<long> IdCategoriaPadre { get; set; }
        public List<ECategoria> children { get; set; }
    }

    public class ECategoriaJSON
    {
        public long IdCategoria { get; set; }
    }

    public class EArticulo
    {
        public long IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public List<ECategoriaJSON> Categoria { get; set; }
    }

}
