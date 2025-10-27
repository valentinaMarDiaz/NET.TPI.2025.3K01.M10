window.renderBarChart = (canvasId, labels, data, title) => {
    if (!window.Chart) return;
    const ctx = document.getElementById(canvasId).getContext('2d');

    if (ctx._chartInstance) { ctx._chartInstance.destroy(); } // limpia si ya había uno

    const chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: title || 'Ventas',
                data: data
            }]
        },
        options: {
            responsive: true,
            scales: { y: { beginAtZero: true } }
        }
    });

    ctx._chartInstance = chart;
};
