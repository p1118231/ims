// Monthly Sales Chart (Bar Chart)
var ctx = document.getElementById('analyticsChart').getContext('2d');
var analyticsChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: months,
        datasets: [{
            label: 'Monthly Sales (£)',
            data: monthlySales,
            backgroundColor: '#E6F0FA', // Light Blue
            borderColor: '#D1E3F6',
            borderWidth: 1
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            y: {
                beginAtZero: true,
                title: {
                    display: true,
                    text: 'Sales (£)',
                    color: '#333333'
                },
                ticks: { color: '#333333' }
            },
            x: {
                title: {
                    display: true,
                    text: 'Months',
                    color: '#333333'
                },
                ticks: { color: '#333333' }
            }
        },
        plugins: {
            legend: { labels: { color: '#333333' } }
        }
    }
});

// Daily Sales Chart (Line Chart)
var dailyCtx = document.getElementById('dailySalesChart').getContext('2d');
var dailySalesChart = new Chart(dailyCtx, {
    type: 'line',
    data: {
        labels: days,
        datasets: [{
            label: 'Daily Sales (£)',
            data: dailySales,
            fill: true,
            backgroundColor: 'rgba(230, 240, 250, 0.3)', // Light Blue fill
            borderColor: '#E6F0FA',
            borderWidth: 2,
            pointBackgroundColor: '#333333',
            pointBorderColor: '#FFFFFF',
            pointRadius: 4,
            tension: 0.3
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            y: {
                beginAtZero: true,
                title: {
                    display: true,
                    text: 'Sales (£)',
                    color: '#333333'
                },
                ticks: { color: '#333333' }
            },
            x: {
                title: {
                    display: true,
                    text: 'Days',
                    color: '#333333'
                },
                ticks: { color: '#333333' }
            }
        },
        plugins: {
            legend: { labels: { color: '#333333' } }
        }
    }
});

// Sales Trend Chart (Area Chart)
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
                        label: 'Sales Trend (£)',
                        data: sales,
                        fill: true,
                        backgroundColor: 'rgba(230, 240, 250, 0.3)', // Light Blue fill
                        borderColor: '#E6F0FA',
                        borderWidth: 2,
                        pointBackgroundColor: '#333333',
                        pointBorderColor: '#FFFFFF',
                        pointRadius: 4,
                        tension: 0.3
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Sales (£)',
                                color: '#333333'
                            },
                            ticks: { color: '#333333' }
                        },
                        x: {
                            title: {
                                display: true,
                                text: 'Date',
                                color: '#333333'
                            },
                            ticks: { color: '#333333' }
                        }
                    },
                    plugins: {
                        legend: { labels: { color: '#333333' } }
                    }
                }
            });
        });
});