using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Entidad;
using Datos;
using Logica;
using Microsoft.Reporting.WebForms;

namespace SistemaERP.Views.Empresa
{
    public partial class FormReporteEmpresa : System.Web.Mvc.ViewPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

               
                   

                        CargarReporte();
                  
              
            }
        }

        public void CargarReporte()
        {
            try
            {

                Usuario usuario = (Usuario)Session["Usuario"];
                List<EREmpresa> empresas = new List<EREmpresa>();
                empresas = LEmpresa.Logica.LEmpresa.ReporteListaEmpresaReportView();

                List<ERDatosBasico> datosBasico = new List<ERDatosBasico>();
                datosBasico = LEmpresa.Logica.LEmpresa.ReporteDatosBasico(usuario.Usuario1);

                ReportDataSource rp = new ReportDataSource("DSReporteEmpresas", empresas);
                ReportDataSource rb = new ReportDataSource("DSReporteBasico", datosBasico);
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteEmpresa.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rp);
                ReportViewer1.LocalReport.DataSources.Add(rb);
                var parameters = ReportViewer1.LocalReport.GetParameters();
                ReportViewer1.LocalReport.Refresh();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}