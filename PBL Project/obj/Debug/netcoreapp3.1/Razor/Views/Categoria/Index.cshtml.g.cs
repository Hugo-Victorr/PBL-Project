#pragma checksum "Z:\SEMESTRE 5\Projetos\PBL Project\Views\Categoria\Index.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3e04de910785a2b18452d2b8f218c1955d2f3ce544ab0cc98b9ef153e892913b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Categoria_Index), @"mvc.1.0.view", @"/Views/Categoria/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Mvc.Rendering;
    using global::Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "Z:\SEMESTRE 5\Projetos\PBL Project\Views\_ViewImports.cshtml"
using PBL_Project;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "Z:\SEMESTRE 5\Projetos\PBL Project\Views\_ViewImports.cshtml"
using PBL_Project.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA256", @"3e04de910785a2b18452d2b8f218c1955d2f3ce544ab0cc98b9ef153e892913b", @"/Views/Categoria/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA256", @"f05e861fbd9fad0fa86667c5a3d9a149dd4e7a4f54e11e6a758ffcd3a7d0838a", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Categoria_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<CategoriaViewModel>>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<a href=\"/categoria/Create\" class=\"btn btn-success\">Novo registro</a>\r\n<br />\r\n<br />\r\n<table class=\"table table-striped table-responsive\">\r\n    <tr>\r\n        <th>Código</th>\r\n        <th>Descrição</th>\r\n        <th>Ações</th>\r\n    </tr>\r\n\r\n");
#nullable restore
#line 13 "Z:\SEMESTRE 5\Projetos\PBL Project\Views\Categoria\Index.cshtml"
     foreach (var categoria in Model)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <tr>\r\n            <td>");
#nullable restore
#line 16 "Z:\SEMESTRE 5\Projetos\PBL Project\Views\Categoria\Index.cshtml"
           Write(categoria.Id);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            <td>");
#nullable restore
#line 17 "Z:\SEMESTRE 5\Projetos\PBL Project\Views\Categoria\Index.cshtml"
           Write(categoria.Descricao);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            <td>\r\n                <a");
            BeginWriteAttribute("href", " href=\"", 451, "\"", 490, 2);
            WriteAttributeValue("", 458, "/categoria/edit?id=", 458, 19, true);
#nullable restore
#line 19 "Z:\SEMESTRE 5\Projetos\PBL Project\Views\Categoria\Index.cshtml"
WriteAttributeValue("", 477, categoria.Id, 477, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">✍🏻</a>\r\n                <a");
            BeginWriteAttribute("href", " href=\"", 519, "\"", 567, 3);
            WriteAttributeValue("", 526, "javascript:apagarCategoria(", 526, 27, true);
#nullable restore
#line 20 "Z:\SEMESTRE 5\Projetos\PBL Project\Views\Categoria\Index.cshtml"
WriteAttributeValue("", 553, categoria.Id, 553, 13, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 566, ")", 566, 1, true);
            EndWriteAttribute();
            WriteLiteral(">💣</a>\r\n            </td>\r\n        </tr>\r\n");
#nullable restore
#line 23 "Z:\SEMESTRE 5\Projetos\PBL Project\Views\Categoria\Index.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</table>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<CategoriaViewModel>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
