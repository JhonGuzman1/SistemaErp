using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class LLogica<E> : LLogicaLogica<dbSistemaErpEntities, E>
    {
        protected override dbSistemaErpEntities GetEsquema()
        {
            return new dbSistemaErpEntities();
        }

        private static LLogica<E> logica;

        private LUsuario lUsuario;
        private LEmpresa lEmpresa;
        private LGestion lGestion;
        private LPeriodo lPeriodo;
        private LCuenta lCuenta;
        private LMoneda lMoneda;
        private LComprobante lComprobante;
        private LCategoria lCategoria;
        private LArticulo lArticulo;
        private LNotaCompra lNotaCompra;
        private LNotaVenta lNotaVenta;


        public static LLogica<E> Logica
        {
            get
            {
                if (logica == null)
                {
                    logica = new LLogica<E>();
                }
                return logica;
            }
        }

        public LUsuario LUsuario
        {
            get
            {
                if (lUsuario == null)
                {
                    lUsuario = new LUsuario();
                }
                return lUsuario;
            }
        }

        public LEmpresa LEmpresa
        {
            get
            {
                if (lEmpresa == null)
                {
                    lEmpresa = new LEmpresa();
                }
                return lEmpresa;
            }
        }

        public LGestion LGestion
        {
            get
            {
                if (lGestion == null)
                {
                    lGestion = new LGestion();
                }
                return lGestion;
            }
        }

        public LPeriodo LPeriodo
        {
            get
            {
                if (lPeriodo == null)
                {
                    lPeriodo = new LPeriodo();
                }
                return lPeriodo;
            }
        }

        public LCuenta LCuenta
        {
            get
            {
                if (lCuenta == null)
                {
                    lCuenta = new LCuenta();
                }
                return lCuenta;
            }
        }

        public LMoneda LMoneda
        {
            get
            {
                if (lMoneda == null)
                {
                    lMoneda = new LMoneda();
                }
                return lMoneda;
            }
        }

        public LComprobante LComprobante
        {
            get
            {
                if (lComprobante == null)
                {
                    lComprobante = new LComprobante();
                }
                return lComprobante;
            }
        }

        public LCategoria LCategoria
        {
            get
            {
                if (lCategoria == null)
                {
                    lCategoria = new LCategoria();
                }
                return lCategoria;
            }
        }

        public LArticulo LArticulo
        {
            get
            {
                if (lArticulo == null)
                {
                    lArticulo = new LArticulo();
                }
                return lArticulo;
            }
        }

        public LNotaCompra LNotaCompra
        {
            get
            {
                if (lNotaCompra == null)
                {
                    lNotaCompra = new LNotaCompra();
                }
                return lNotaCompra;
            }
        }

        public LNotaVenta LNotaVenta
        {
            get
            {
                if (lNotaVenta == null)
                {
                    lNotaVenta = new LNotaVenta();
                }
                return lNotaVenta;
            }
        }
    }
}
