var ctx = document.getElementById('analyticsChart').getContext('2d');
        var analyticsChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: months,
                datasets: [
                    {
                        label: 'Actual Sales',
                        data: monthlySales,
                        backgroundColor: 'rgba(75, 192, 192, 0.2)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1
                    }
                ]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });

        var dailyCtx = document.getElementById('dailySalesChart').getContext('2d');
        var dailySalesChart = new Chart(dailyCtx, {
            type: 'line',
            data: {
            labels: days,
            datasets: [
                {
                label: 'Daily Sales',
                data: dailySales,
                backgroundColor: 'rgba(153, 102, 255, 0.2)',
                borderColor: 'rgba(153, 102, 255, 1)',
                borderWidth: 1
                }
            ]
            },
            options: {
            scales: {
                y: {
                beginAtZero: true
                }
            }
            }
        });

        document.addEventListener('DOMContentLoaded', function () {
            const ctx = document.getElementById('salesChart').getContext('2d');
            fetch('/Analytics/SalesTrends')
                .then(response => response.json())
                .then(data => {
                    const dates = data.map(d => d.Date);
                    const sales = data.map(d => d.TotalSales);
        
                    new Chart(ctx, {
                        type: 'line',
                        data: {
                            labels: dates,
                            datasets: [{
                                label: 'Daily Sales',
                                data: sales,
                                borderColor: 'rgb(75, 192, 192)',
                                tension: 0.1
                            }]
                        },
                        options: {
                            scales: {
                                y: {
                                    beginAtZero: true,
                                    title: {
                                        display: true,
                                        text: 'Sales Amount ($)'
                                    }
                                },
                                x: {
                                    title: {
                                        display: true,
                                        text: 'Date'
                                    }
                                }
                            }
                        }
                    });
                });
        });