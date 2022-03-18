using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class MensageException: Exception
    {

        public int Codigo { get; set; }

        public MensageException(int Codigo, string Mensaje) : base(Mensaje)
        {
            this.Codigo = Codigo;
        }

        public MensageException(string Mensaje)
            : base(Mensaje)
        {
            this.Codigo = 1;
        }

    }
}
