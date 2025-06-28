
$(document).ready(function () {
    $.ajax({
        url: '/GalLabs/GetVendasNoAnoData',
        type: 'GET',
        success: function (data) {
            // Verifica se os arrays têm 12 meses
            console.log(data);

            var linechartcustomerColors = getChartColorsArray("vendas_no_ano");
            if (linechartcustomerColors) {
                var options = {
                    series: [
                        {
                            name: "Pagos",
                            type: "area",
                            data: data.pagos,
                        },
                        {
                            name: "Processando",
                            type: "bar",
                            data: data.emProcessamento,
                        },
                        {
                            name: "Cancelados",
                            type: "line",
                            data: data.cancelados,
                        },
                    ],
                    chart: {
                        height: 370,
                        type: "line",
                        toolbar: {
                            show: false,
                        },
                    },
                    stroke: {
                        curve: "straight",
                        dashArray: [0, 0, 8],
                        width: [2, 0, 2.2],
                    },
                    fill: {
                        opacity: [0.1, 0.9, 1],
                    },
                    markers: {
                        size: [0, 0, 0],
                        strokeWidth: 2,
                        hover: {
                            size: 4,
                        },
                    },
                    xaxis: {
                        categories: [
                            "Jan", "Fev", "Mar", "Abr", "Mai", "Jun",
                            "Jul", "Ago", "Set", "Out", "Nov", "Dez"
                        ],
                        axisTicks: {
                            show: false,
                        },
                        axisBorder: {
                            show: false,
                        },
                    },
                    grid: {
                        show: true,
                        xaxis: {
                            lines: {
                                show: true,
                            },
                        },
                        yaxis: {
                            lines: {
                                show: false,
                            },
                        },
                        padding: {
                            top: 0,
                            right: -2,
                            bottom: 15,
                            left: 10,
                        },
                    },
                    legend: {
                        show: true,
                        horizontalAlign: "center",
                        offsetX: 0,
                        offsetY: -5,
                        markers: {
                            width: 9,
                            height: 9,
                            radius: 6,
                        },
                        itemMargin: {
                            horizontal: 10,
                            vertical: 0,
                        },
                    },
                    plotOptions: {
                        bar: {
                            columnWidth: "30%",
                            barHeight: "70%",
                        },
                    },
                    colors: linechartcustomerColors,
                    tooltip: {
                        shared: true,
                        y: [{
                            formatter: function (y) {
                                if (typeof y !== "undefined") {
                                    return "R$ " + y.toFixed(2);
                                }
                                return y;
                            },
                        },
                        {
                            formatter: function (y) {
                                if (typeof y !== "undefined") {
                                    return "R$ " + y.toFixed(2);// + "k";
                                }
                                return y;
                            },
                        },
                        {
                            formatter: function (y) {
                                if (typeof y !== "undefined") {
                                    return "R$ " + y.toFixed(2);// + " Vendas";
                                }
                                return y;
                            },
                        },
                        ],
                    },
                };
                var chart = new ApexCharts(
                    document.querySelector("#vendas_no_ano"),
                    options
                );
                chart.render();
            }
        },
        error: function (xhr, status, error) {
            console.error('Erro ao carregar os dados:', error);
        }
    });
});



// FIM DO 1º GRÁFICO: VENDAS NO ANO, POR MÊS


// INÍCIO DO 2º GRÁFICO: TOP VENDEDORES
$(document).ready(function () {
    $.ajax({
        url: '/GalLabs/GetTop10VendedoresData',
        type: 'GET',
        success: function (data) {
            console.log("Dados Recebidos:", data);  // Verificando dados no console

            var barchartCountriesColors = getChartColorsArray("topten_vendedores");
            if (barchartCountriesColors) {
                var options = {
                    series: [{
                        name: 'Qtde Vendas',
                        data: data.map(vendedor => vendedor.quantidadeVendas)  // Mapeando quantidade de vendas
                    }],
                    chart: {
                        type: 'bar',
                        height: 436,
                        toolbar: {
                            show: false,
                        }
                    },
                    plotOptions: {
                        bar: {
                            borderRadius: 4,
                            horizontal: true,
                            distributed: true,
                            dataLabels: {
                                position: 'top',
                            },
                        }
                    },
                    colors: barchartCountriesColors,
                    dataLabels: {
                        enabled: true,
                        offsetX: 32,
                        style: {
                            fontSize: '12px',
                            fontWeight: 400,
                            colors: ['#adb5bd']
                        }
                    },
                    legend: {
                        show: false,
                    },
                    grid: {
                        show: false,
                    },
                    xaxis: {
                        categories: data.map(vendedor => vendedor.nomeVendedor),  // Mapeando nomes dos vendedores
                    },
                };

                var chart = new ApexCharts(document.querySelector("#topten_vendedores"), options);
                chart.render();
            }
        },
        error: function (xhr, status, error) {
            console.error('Erro ao carregar os dados:', error);
        }
    });
});

// FIM DO 2º GRÁFICO