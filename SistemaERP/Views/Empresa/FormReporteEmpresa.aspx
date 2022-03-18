<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormReporteEmpresa.aspx.cs" Inherits="SistemaERP.Views.Empresa.FormReporteEmpresa" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
      <asp:ScriptManager ID="ScriptManager1" runat="server">
</asp:ScriptManager>
       
          
    
     
          
    
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%">
        </rsweb:ReportViewer>
       
          
    
    </form>
</body>
</html>
