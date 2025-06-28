$(document).ready(function () {
    //$(window).on('load', function() { //testar se dá certo
    $.ajax({
        url: '/GalLabs/GetEstadosPedidoDonutData',
        type: 'GET',
        success: function (data) {
            // Verifica se os arrays têm 12 meses
            console.log("os dados retornados do donut", data);
            //Gráfico Original------------------------
            var donutchartportfolioColors = getChartColorsArray("portfolio_donut_chart");
            if (donutchartportfolioColors) {
                var options = {
                    series: [Number(data.valorOrcamento), Number(data.valorEmProcessamento), Number(data.valorCancelado), Number(data.valorEntregue)],
                    labels: ["Orçamento", "Processamento", "Cancelados", "Entregues"],
                    chart: {
                        type: "donut",
                        height: 210,
                    },

                    plotOptions: {
                        pie: {
                            size: 100,
                            offsetX: 0,
                            offsetY: 0,
                            donut: {
                                size: "70%",
                                labels: {
                                    show: true,
                                    name: {
                                        show: true,
                                        fontSize: '18px',
                                        offsetY: -5,
                                    },
                                    value: {
                                        show: true,
                                        fontSize: '14px',
                                        color: '#343a40',
                                        fontWeight: 350,
                                        offsetY: 5,
                                        formatter: function (val) {
                                            return "R$ " + val
                                        }
                                    },
                                    total: {
                                        show: true,
                                        fontSize: '13px',
                                        label: 'Total value',
                                        color: '#9599ad',
                                        fontWeight: 500,
                                        formatter: function (w) {
                                            return "R$ " + w.globals.seriesTotals.reduce(function (a, b) {
                                                return a + b
                                            }, 0)
                                        }
                                    }
                                }
                            },
                        },
                    },
                    dataLabels: {
                        enabled: false,
                    },
                    legend: {
                        show: false,
                    },
                    yaxis: {
                        labels: {
                            formatter: function (value) {
                                return "$" + value;
                            }
                        }
                    },
                    stroke: {
                        lineCap: "round",
                        width: 2
                    },
                    colors: donutchartportfolioColors,
                };

                var chart = new ApexCharts(document.querySelector("#portfolio_donut_chart"), options);
                chart.render();
            }

        }

    })
})