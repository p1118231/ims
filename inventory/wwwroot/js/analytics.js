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
document.addEventListener('DOMContentLoaded', function () {
    // Sales Trend Chart (Enhanced Area Chart)
    const salesCtx = document.getElementById('salesChart').getContext('2d');
    if (salesTrend && salesTrend.length > 0) {
        const dates = salesTrend.map(d => d.date);
        const sales = salesTrend.map(d => d.totalSales);

        new Chart(salesCtx, {
            type: 'line',
            data: {
                labels: dates,
                datasets: [{
                    label: 'Sales Trend (£)',
                    data: sales,
                    fill: {
                        target: 'origin',
                        above: 'rgba(230, 240, 250, 0.5)', // Light Blue gradient fill
                        below: 'rgba(230, 240, 250, 0.1)'
                    },
                    backgroundColor: 'rgba(230, 240, 250, 0.5)',
                    borderColor: '#E6F0FA',
                    borderWidth: 3,
                    pointBackgroundColor: '#333333',
                    pointBorderColor: '#FFFFFF',
                    pointRadius: 5,
                    pointHoverRadius: 8,
                    tension: 0.4 // Smooth curve
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
                            color: '#333333',
                            font: { size: 14, weight: 'bold' }
                        },
                        ticks: { 
                            color: '#333333',
                            callback: value => '£' + value.toLocaleString('en-GB')
                        },
                        grid: { color: '#E6F0FA', lineWidth: 1 }
                    },
                    x: {
                        title: {
                            display: true,
                            text: 'Date',
                            color: '#333333',
                            font: { size: 14, weight: 'bold' }
                        },
                        ticks: { 
                            color: '#333333',
                            maxRotation: 45,
                            minRotation: 45,
                            maxTicksLimit: 10 // Limit x-axis labels for readability
                        },
                        grid: { display: false }
                    }
                },
                plugins: {
                    legend: { 
                        labels: { 
                            color: '#333333',
                            font: { size: 14 }
                        }
                    },
                    tooltip: {
                        backgroundColor: '#333333',
                        titleColor: '#FFFFFF',
                        bodyColor: '#FFFFFF',
                        callbacks: {
                            label: context => `Sales: £${context.parsed.y.toLocaleString('en-GB')}`
                        }
                    }
                },
                animation: {
                    duration: 1500,
                    easing: 'easeOutQuart'
                },
                hover: {
                    mode: 'nearest',
                    intersect: true
                }
            }
        });
    } else {
        console.warn('No sales trend data available to render chart');
        salesCtx.font = '16px Arial';
        salesCtx.fillStyle = '#666666';
        salesCtx.textAlign = 'center';
        salesCtx.fillText('No sales data available', salesCtx.canvas.width / 2, salesCtx.canvas.height / 2);
    }

    // Monthly Sales Chart (Bar Chart)
    const monthlyCtx = document.getElementById('analyticsChart').getContext('2d');
    new Chart(monthlyCtx, {
        type: 'bar',
        data: {
            labels: months,
            datasets: [{
                label: 'Monthly Sales (£)',
                data: monthlySales,
                backgroundColor: '#E6F0FA',
                borderColor: '#D1E3F6',
                borderWidth: 1,
                borderRadius: 4
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: { 
                    beginAtZero: true, 
                    ticks: { callback: value => '£' + value.toLocaleString('en-GB') },
                    grid: { color: '#E6F0FA' }
                },
                x: { grid: { display: false } }
            },
            plugins: {
                tooltip: { callbacks: { label: context => `£${context.parsed.y.toLocaleString('en-GB')}` } }
            }
        }
    });

    // Daily Sales Chart (Line Chart)
    const dailyCtx = document.getElementById('dailySalesChart').getContext('2d');
    new Chart(dailyCtx, {
        type: 'line',
        data: {
            labels: days,
            datasets: [{
                label: 'Daily Sales (£)',
                data: dailySales,
                fill: false,
                borderColor: '#E6F0FA',
                borderWidth: 2,
                pointBackgroundColor: '#333333',
                pointRadius: 4,
                tension: 0.2
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: { 
                    beginAtZero: true, 
                    ticks: { callback: value => '£' + value.toLocaleString('en-GB') },
                    grid: { color: '#E6F0FA' }
                },
                x: { grid: { display: false } }
            },
            plugins: {
                tooltip: { callbacks: { label: context => `£${context.parsed.y.toLocaleString('en-GB')}` } }
            }
        }
    });
});
const categoryCtx = document.getElementById('categorySalesChart').getContext('2d');
            if (categorySales && categorySales.length > 0) {
                const categories = categorySales.map(c => c.category);
                const sales = categorySales.map(c => c.sales);
                new Chart(categoryCtx, {
                    type: 'pie',
                    data: {
                        labels: categories,
                        datasets: [{
                            data: sales,
                            backgroundColor: ['#E6F0FA', '#D1E3F6', '#A9C7E8', '#82AADC', '#5B8DD0'], // Shades of blue
                            borderColor: '#FFFFFF',
                            borderWidth: 2
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                position: 'right',
                                labels: { color: '#333333', font: { size: 14 } }
                            },
                            tooltip: {
                                backgroundColor: '#333333',
                                titleColor: '#FFFFFF',
                                bodyColor: '#FFFFFF',
                                callbacks: {
                                    label: context => `${context.label}: £${context.parsed.toLocaleString('en-GB')}`
                                }
                            }
                        },
                        animation: {
                            duration: 1500,
                            easing: 'easeOutBounce'
                        }
                    }
                });
            } else {
                categoryCtx.font = '16px Arial';
                categoryCtx.fillStyle = '#666666';
                categoryCtx.textAlign = 'center';
                categoryCtx.fillText('No category sales data available', categoryCtx.canvas.width / 2, categoryCtx.canvas.height / 2);
            }
        