//// Gráficos dinâmicos por Quantidade e por Valor, Trimestral, ou Quadrimestral, ou Semestral.

$(document).ready(function () {
    // Função para obter as cores do gráfico
    function getColorsArray(id) {
        return getChartColorsArray(id);
    }

    // Função para configurar opções de label
    function getLabelOption(config) {
        return {
            show: true,
            position: config.position,
            distance: config.distance,
            align: config.align,
            verticalAlign: config.verticalAlign,
            rotate: config.rotate,
            formatter: config.formatter || '{c}  {name|{a}}',
            fontSize: 16,
            rich: { name: {} }
        };
    }

    // Função para configurar a toolbox
    function getToolbox() {
        return {
            show: true,
            orient: 'vertical',
            left: 'right',
            top: 'center',
            feature: {
                mark: { show: true },
                dataView: { show: true, readOnly: false },
                magicType: { show: true, type: ['line', 'bar', 'stack'] },
                restore: { show: true },
                saveAsImage: { show: true }
            }
        };
    }

    // Função para configurar eixo X e Y
    function getAxis(periodoLabels) {
        return {
            xAxis: [{
                type: 'category',
                axisTick: { show: false },
                data: periodoLabels,
                axisLine: { lineStyle: { color: '#858d98' } }
            }],
            yAxis: {
                type: 'value',
                height: 436,
                axisLine: { lineStyle: { color: '#858d98' } },
                splitLine: { lineStyle: { color: "rgba(133, 141, 152, 0.1)" } }
            }
        };
    }

    // Função para gerar o gráfico
    function generateChart(domId, url, formatterFn, ano, periodo) {
        var colors = getColorsArray(domId);
        if (!colors) return;

        var chartDom = document.getElementById(domId);
        var myChart = echarts.init(chartDom);

        var posList = ['left', 'right', 'top', 'bottom', 'inside', 'insideTop', 'insideLeft', 'insideRight', 'insideBottom', 'insideTopLeft', 'insideTopRight', 'insideBottomLeft', 'insideBottomRight'];
        var appConfig = {
            rotate: 90,
            align: 'left',
            verticalAlign: 'middle',
            position: 'insideBottom',
            distance: 15
        };

        var labelOption = getLabelOption(appConfig);

        $.ajax({
            url: url,
            type: 'GET',
            data: {
                ano: ano,
                periodo: periodo
            }, // Passa os parâmetros ano e período aqui
            success: function (data) {
                //console.log("Retorno JSON da API:", data);

                var clientesNames = [];
                var periodoLabels = ['Trimestre 1', 'Trimestre 2', 'Trimestre 3', 'Trimestre 4']; // Trimestral
                if (data.length === 3) {
                    periodoLabels = ['Quadrimestre 1', 'Quadrimestre 2', 'Quadrimestre 3']; // Quadrimestral
                } else if (data.length === 2) {
                    periodoLabels = ['Semestre 1', 'Semestre 2']; // Semestral
                }

                data.forEach(function (periodoData) {
                    if (periodoData.clientes && Array.isArray(periodoData.clientes)) {
                        periodoData.clientes.forEach(function (cliente, index) {
                            if (!clientesNames[index]) {
                                clientesNames[index] = {
                                    name: cliente.nomeCliente,
                                    type: 'bar',
                                    label: labelOption,
                                    barGap: 0,
                                    emphasis: { focus: 'series' },
                                    data: []
                                };
                            }
                            clientesNames[index].data.push(formatterFn(cliente));
                        });
                    }
                });

                var option = {
                    grid: { left: '0%', right: '0%', bottom: '0%', containLabel: true },
                    tooltip: {
                        trigger: 'axis',
                        axisPointer: { type: 'shadow' }
                    },
                    legend: {
                        data: clientesNames.map(cliente => cliente.name),
                        textStyle: { color: '#858d98' }
                    },
                    toolbox: getToolbox(),
                    ...getAxis(periodoLabels),
                    color: colors,
                    series: clientesNames
                };

                myChart.setOption(option);
            },
            error: function (xhr, status, error) {
                console.error('Erro ao carregar os dados:', error);
            }
        });
    }

    // Gráfico por Quantidade (Semestral por padrão)
    function loadChartQuantidade() {

        var ano = $('#ano-select-quantidade').val() || 2024; // 2024 como padrão
        var periodo = $('#periodo-select-quantidade').val() || 6; // Semestral por default


        generateChart(
            "chart-bar-label-rotation-glsolutions",
            '/GalLabs/GetTop4ClientesPorPeriodoQuantidadeData',
            function (cliente) {
                return cliente.quantidadePedidos;
            },
            ano,  // ano selecionado
            periodo  // período selecionado
        );

    }

    // Gráfico por Valor (Quadrimestral por padrão)
    function loadChartValores() {
        generateChart(
            "chart-bar-label-rotation-glsolutions-por-valores",
            '/GalLabs/GetTop4ClientesPorPeriodoValorData',
            function (cliente) {
                return parseFloat(cliente.valorTotal);
            },
            $('#ano-select-valores').val() || 2024,  // Obtendo o ano selecionado ou 2024
            $('#periodo-select-valores').val() || 4  // Quadrimestral por padrão
        );
    }

    // Carrega os gráficos pela primeira vez
    loadChartQuantidade();
    loadChartValores();

    // Quando o ano ou o periodo for alterado, recarrega o gráfico de Quantidade
    $('#ano-select-quantidade, #periodo-select-quantidade').change(function () {
        loadChartQuantidade();
    });

    // Quando o ano ou o periodo for alterado, recarrega o gráfico de Valor
    $('#ano-select-valores, #periodo-select-valores').change(function () {
        loadChartValores();
    });

});

