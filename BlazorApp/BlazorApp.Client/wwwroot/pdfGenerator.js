function generarPdfHistorialPrecios(nombreProducto, historialData) {
    // Asegúrate de que jsPDF esté cargado
    if (typeof jspdf === 'undefined') {
        console.error("jsPDF no está cargado.");
        alert("Error: La librería PDF no se pudo cargar.");
        return;
    }
    const { jsPDF } = jspdf; // Extraer el constructor

    // Crear instancia del PDF
    const doc = new jsPDF();

    // Título del reporte
    doc.setFontSize(18);
    doc.text(`Historial de Precios - ${nombreProducto}`, 14, 22);

    // Preparar datos para la tabla
    const head = [['Fecha y Hora (Local)', 'Precio Registrado']];
    const body = historialData.map(h => [
        new Date(h.fechaModificacionUtc).toLocaleString(), // Formatear fecha a local
        // Formatear como moneda ARS (Peso Argentino)
        parseFloat(h.valor).toLocaleString('es-AR', { style: 'currency', currency: 'ARS' })
    ]);

    // Agregar la tabla al documento usando autoTable
    if (doc.autoTable) {
        doc.autoTable({
            head: head,
            body: body,
            startY: 30, // Posición vertical donde empieza la tabla
            headStyles: {
                fillColor: [41, 128, 185], // Mantenemos el color azul del encabezado
                // Quitamos la alineación específica de aquí
            },
            styles: { fontSize: 10 },
            // ---------- INICIO CORRECCIÓN ----------
            columnStyles: {
                0: { cellWidth: 'auto' }, // Columna Fecha (índice 0) - Alineación por defecto (izquierda)
                1: { // Columna Precio (índice 1)
                    cellWidth: 'auto',
                    halign: 'right' // Aplicar alineación derecha a TODA la columna (header y body)
                }
            }
            // ---------- FIN CORRECCIÓN ----------
        });
    } else {
        // Fallback si autoTable no está disponible
        console.warn("jsPDF-AutoTable no está cargado. Mostrando datos como texto simple.");
        let y = 30;
        doc.setFontSize(12);
        // Encabezado (intentar alinear manualmente, aproximado)
        doc.text(head[0][0], 14, y); // Fecha a la izquierda
        let precioHeaderWidth = doc.getTextWidth(head[0][1]);
        let pageWidth = doc.internal.pageSize.getWidth();
        doc.text(head[0][1], pageWidth - 14 - precioHeaderWidth, y); // Precio a la derecha
        y += 7;

        // Cuerpo (intentar alinear manualmente, aproximado)
        body.forEach(row => {
            doc.text(row[0], 14, y); // Fecha a la izquierda
            let precioWidth = doc.getTextWidth(row[1]);
            doc.text(row[1], pageWidth - 14 - precioWidth, y); // Precio a la derecha
            y += 7;
        });
    }


    // Guardar el PDF
    doc.save(`HistorialPrecios_${nombreProducto.replace(/ /g, '_')}.pdf`);
}

function generarPdfVentas(ventasData, filtroClienteId, filtroDesde, filtroHasta) {
    // Asegúrate de que jsPDF y autoTable estén cargados
    if (typeof jspdf === 'undefined' || typeof jspdf.jsPDF === 'undefined') {
        console.error("jsPDF no está cargado.");
        alert("Error: La librería PDF no se pudo cargar.");
        return;
    }
    const { jsPDF } = jspdf;

    if (typeof jsPDF.API.autoTable !== 'function') {
        console.error("jsPDF-AutoTable no está cargado.");
        alert("Error: La librería para tablas PDF no se pudo cargar.");
        return;
    }

    // Crear instancia del PDF
    const doc = new jsPDF({
        orientation: "landscape" // Orientación horizontal para más espacio
    });

    // Título del reporte
    doc.setFontSize(18);
    doc.text("Reporte de Ventas", 14, 22);

    // Subtítulo con filtros aplicados (si existen)
    doc.setFontSize(10);
    let filtroTexto = "Filtros aplicados: ";
    let filtrosAplicados = false;
    if (filtroClienteId) {
        filtroTexto += `Cliente ID: ${filtroClienteId}`;
        filtrosAplicados = true;
    }
    if (filtroDesde) {
        filtroTexto += (filtrosAplicados ? " | " : "") + `Desde: ${new Date(filtroDesde).toLocaleDateString()}`;
        filtrosAplicados = true;
    }
    if (filtroHasta) {
        filtroTexto += (filtrosAplicados ? " | " : "") + `Hasta: ${new Date(filtroHasta).toLocaleDateString()}`;
        filtrosAplicados = true;
    }
    if (!filtrosAplicados) {
        filtroTexto += "Ninguno";
    }
    doc.text(filtroTexto, 14, 30);


    // Preparar datos para la tabla
    const head = [['ID Venta', 'Cliente', 'ID Cliente', 'Fecha (Local)', 'Total']];
    const body = ventasData.map(v => [
        v.idVenta,
        v.clienteNombre,
        v.idCliente,
        new Date(v.fechaHoraVentaUtc).toLocaleString(), // Formatear fecha a local
        // Formatear como moneda ARS (Peso Argentino) - Usamos totalListado
        parseFloat(v.totalListado).toLocaleString('es-AR', { style: 'currency', currency: 'ARS' })
    ]);

    // Agregar la tabla al documento usando autoTable
    doc.autoTable({
        head: head,
        body: body,
        startY: 38, // Posición vertical donde empieza la tabla (debajo de los títulos)
        headStyles: {
            fillColor: [41, 128, 185], // Color azul para encabezado
            halign: 'center' // Centrar encabezados
        },
        styles: { fontSize: 9 }, // Tamaño de fuente más pequeño para que quepa mejor
        columnStyles: {
            0: { halign: 'center' }, // ID Venta centrado
            1: { halign: 'left' },   // Cliente a la izquierda
            2: { halign: 'center' }, // ID Cliente centrado
            3: { halign: 'center' }, // Fecha centrado
            4: { halign: 'right' }   // Total a la derecha
        }
    });

    // Guardar el PDF
    let nombreArchivo = "ReporteVentas";
    if (filtroDesde) nombreArchivo += `_Desde_${new Date(filtroDesde).toLocaleDateString().replace(/\//g, '-')}`;
    if (filtroHasta) nombreArchivo += `_Hasta_${new Date(filtroHasta).toLocaleDateString().replace(/\//g, '-')}`;
    if (filtroClienteId) nombreArchivo += `_Cliente_${filtroClienteId}`;
    doc.save(`${nombreArchivo}.pdf`);
}