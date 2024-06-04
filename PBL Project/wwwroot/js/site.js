function apagarEmpresa(id) {
    if (confirm('Confirma a exclusão do registro?'))
        location.href = '/empresa/Delete?id=' + id;
}

function apagarUnidade(id) {
    if (confirm('Confirma a exclusão do registro?'))
        location.href = '/unidade/DeleteUnidade?id=' + id;
}

function apagarDispositivo(device_id, id) {
    if (confirm('Confirma a exclusão do registro?'))
        location.href = '/dispositivo/DeleteDispositivo?device_id=' + device_id + '&id=' + id;
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

function aplicaFiltroConsultaAvancadaUnidades() {
    var vDescricao = document.getElementById('descricao').value;
    var vEmpresa = document.getElementById('empresa').value;
    var vCategoria = document.getElementById('categoria').value;
    var vEstado = document.getElementById('estado').value;
    $.ajax({
        url: "/unidade/ObtemDadosConsultaAvancada",
        data: { descricao: vDescricao, empresaId: vEmpresa, categoriaId: vCategoria, estadoId: vEstado},
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

function aplicaFiltroConsultaAvancadaDispositivos() {
    var vDescricao = document.getElementById('descricao').value;
    var vEmpresa = document.getElementById('empresa').value;
    var vUnidade = document.getElementById('unidade').value;
    var vCategoria = document.getElementById('categoria').value;
    var vEstado = document.getElementById('estado').value;
    $.ajax({
        url: "/dispositivo/ObtemDadosConsultaAvancada",
        data: { descricao: vDescricao, empresaId: vEmpresa, unidadeId: vUnidade, categoriaId: vCategoria, estadoId: vEstado },
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

function criaTabelaLeituras() {
    $.ajax({
        url: "/dispositivo/ObtemDadosLeituras",
        success: function (dados) {
            if (dados.erro != undefined) {
                alert(dados.msg);
            }
            else {
                document.getElementById('resultadoLeituras').innerHTML = dados;
            }
        },
    });
}

function fetchDadosParaGrafico(id) {
    fetch(`/dispositivo/ExecutarTarefaPeriodica?id=${id}`)
        .then(response => response.json()
        .then(data => {
            createTemperatureChart(data.tempos, data.temperaturas, data.erroRelativo);
        })
        .catch(error => console.error('Erro:', error)));
}

let myChart; 

function createTemperatureChart(tempos, temperaturas, erroRelativo) {
    const ctx = document.getElementById('grafico').getContext('2d');

    if (myChart) {
        myChart.destroy();
    }

    myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: tempos,
            datasets: [{
                label: 'Temperatura',
                data: temperaturas,
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                borderWidth: 1
            },
            {
                label: 'Erro relativo',
                data: erroRelativo,
                borderColor: 'rgba(255, 0, 0, 1)',
                backgroundColor: 'rgba(255, 0, 0, 0.2)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Tempo'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Temperatura (°C)'
                    },
                    beginAtZero: true
                }
            }
        }
    });
}

//cria graficos dashboard
function consultaAvancadaEmpresaUnidade(filtro) {
    fetch(`/dispositivo/ObtemDadosGraficos?filtro=${filtro}`)
        .then(response => response.json()
            .then(data => {
                createPieChart(data.labels, data.dispositivos, data.chartColors);
            })
            .catch(error => console.error('Erro:', error)));
}
function consultaAvancadaCategoriaEstado(filtro) {
    fetch(`/dispositivo/ObtemDadosGraficos?filtro=${filtro}`)
        .then(response => response.json()
            .then(data => {
                createBarChart(data.labels, data.dispositivos, data.chartColors);
            })
            .catch(error => console.error('Erro:', error)));
}


let myPieChart;
function createPieChart(labels, quantidade, pieColors) {
    const ctx = document.getElementById('graficoPieEmpresaUnidade').getContext('2d');

    if (myPieChart) {
        myPieChart.destroy();
    }

    myPieChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: quantidade,
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: pieColors,
                borderWidth: 1
            }]
        },
        options: {
            legend: {
                display: false
            },
            layout: {
                padding: {
                    left: 20,
                    right: 20,
                    top: 50, 
                    bottom: 20
                }
            },
            plugins: {
                legend: {
                    position: 'top', 
                    labels: {
                        boxWidth: 20,
                        font: {
                            size: 12 
                        }
                    }
                }
            },
            responsive: true,
        }
    });
}

let myBarChart;

function createBarChart(labels, quantidade, barColors) {
    const ctx = document.getElementById('graficoBarCategoriaEstado').getContext('2d');

    if (myBarChart) {
        myBarChart.destroy();
    }

    myBarChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                data: quantidade,
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: barColors,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    title: {
                        display: true,
                        text: 'Quantidade de Dispositivos'
                    },
                    beginAtZero: true
                }
            }
        }
    });
}





