function apagarEmpresa(id) {
    if (confirm('Confirma a exclusão do registro?'))
        location.href = '/empresa/Delete?id=' + id;
}

function apagarUnidade(id) {
    if (confirm('Confirma a exclusão do registro?'))
        location.href = '/unidade/Delete?id=' + id;
}

function apagarDispositivo(id) {
    if (confirm('Confirma a exclusão do registro?'))
        location.href = '/dispositivo/Delete?id=' + id;
}

function apagarCategoria(id) {
    if (confirm('Confirma a exclusão do registro?'))
        location.href = '/categoria/Delete?id=' + id;
}

function apagarEstado(id) {
    if (confirm('Confirma a exclusão do registro?'))
        location.href = '/estado/Delete?id=' + id;
}

function exibirImagem() {
    var oFReader = new FileReader();
    oFReader.readAsDataURL(document.getElementById("Imagem").files[0]);
    oFReader.onload = function (oFREvent) {
        document.getElementById("imgPreview").src = oFREvent.target.result;
    };
}

function aplicaFiltroConsultaAvancadaEmpresas() {
    var vDescricao = document.getElementById('descricao').value;
    var vEstado = document.getElementById('estado').value;
    var vCategoria = document.getElementById('categoria').value;
    $.ajax({
        url: "/empresa/ObtemDadosConsultaAvancada",
        data: { descricao: vDescricao, categoriaId: vCategoria, estadoId: vEstado},
        success: function (dados) {
            if (dados.erro != undefined) {
                alert(dados.msg);
            }
            else {
                document.getElementById('resultadoConsulta').innerHTML = dados;
            }
        },
    });
}

//function aplicaFiltroConsultaAvancadaUnidades() {
//    var vDescricao = document.getElementById('descricao').value;
//    var vCategoria = document.getElementById('categoria').value;
//    var vDataInicial = document.getElementById('dataInicial').value;
//    var vDataFinal = document.getElementById('dataFinal').value;
//    $.ajax({
//        url: "/jogo/ObtemDadosConsultaAvancada",
//        data: { descricao: vDescricao, categoria: vCategoria, dataInicial: vDataInicial, dataFinal: vDataFinal },
//        success: function (dados) {
//            if (dados.erro != undefined) {
//                alert(dados.msg);
//            }
//            else {
//                document.getElementById('resultadoConsulta').innerHTML = dados;
//            }
//        },
//    });
//}

//function aplicaFiltroConsultaAvancadaDispositivos() {
//    var vDescricao = document.getElementById('descricao').value;
//    var vCategoria = document.getElementById('categoria').value;
//    var vDataInicial = document.getElementById('dataInicial').value;
//    var vDataFinal = document.getElementById('dataFinal').value;
//    $.ajax({
//        url: "/jogo/ObtemDadosConsultaAvancada",
//        data: { descricao: vDescricao, categoria: vCategoria, dataInicial: vDataInicial, dataFinal: vDataFinal },
//        success: function (dados) {
//            if (dados.erro != undefined) {
//                alert(dados.msg);
//            }
//            else {
//                document.getElementById('resultadoConsulta').innerHTML = dados;
//            }
//        },
//    });
//}



