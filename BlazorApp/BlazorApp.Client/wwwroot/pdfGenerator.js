function generarPdfHistorialPrecios(nombreProducto, historialData) {
    if (typeof jspdf === 'undefined') {
        console.error("jsPDF no está cargado.");
        alert("Error: La librería PDF no se pudo cargar.");
        return;
    }
    const { jsPDF } = jspdf; 

   
    const doc = new jsPDF();

  
    doc.setFontSize(18);
    doc.text(`Historial de Precios - ${nombreProducto}`, 14, 22);

    
    const head = [['Fecha y Hora (Local)', 'Precio Registrado']];
    const body = historialData.map(h => [
        new Date(h.fechaModificacionUtc).toLocaleString(), 
        
        parseFloat(h.valor).toLocaleString('es-AR', { style: 'currency', currency: 'ARS' })
    ]);

    
    if (doc.autoTable) {
        doc.autoTable({
            head: head,
            body: body,
            startY: 30, 
            headStyles: {
                fillColor: [41, 128, 185], 
                
            },
            styles: { fontSize: 10 },
           
            columnStyles: {
                0: { cellWidth: 'auto' }, 
                1: { 
                    cellWidth: 'auto',
                    halign: 'right' 
                }
            }
           
        });
    } else {
        
        console.warn("jsPDF-AutoTable no está cargado. Mostrando datos como texto simple.");
        let y = 30;
        doc.setFontSize(12);
        
        doc.text(head[0][0], 14, y); 
        let precioHeaderWidth = doc.getTextWidth(head[0][1]);
        let pageWidth = doc.internal.pageSize.getWidth();
        doc.text(head[0][1], pageWidth - 14 - precioHeaderWidth, y); 
        y += 7;

        
        body.forEach(row => {
            doc.text(row[0], 14, y); 
            let precioWidth = doc.getTextWidth(row[1]);
            doc.text(row[1], pageWidth - 14 - precioWidth, y); 
            y += 7;
        });
    }



    doc.save(`HistorialPrecios_${nombreProducto.replace(/ /g, '_')}.pdf`);
}

function generarPdfVentas(ventasData, filtroClienteId, filtroDesde, filtroHasta) {
    
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

    const doc = new jsPDF({
        orientation: "landscape" 
    });

   
    doc.setFontSize(18);
    doc.text("Reporte de Ventas", 14, 22);

    
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


    
    const head = [['ID Venta', 'Cliente', 'ID Cliente', 'Fecha (Local)', 'Total']];
    const body = ventasData.map(v => [
        v.idVenta,
        v.clienteNombre,
        v.idCliente,
        new Date(v.fechaHoraVentaUtc).toLocaleString(), 
        
        parseFloat(v.totalListado).toLocaleString('es-AR', { style: 'currency', currency: 'ARS' })
    ]);

    
    doc.autoTable({
        head: head,
        body: body,
        startY: 38, 
        headStyles: {
            fillColor: [41, 128, 185],
            halign: 'center' 
        },
        styles: { fontSize: 9 },
        columnStyles: {
            0: { halign: 'center' }, 
            1: { halign: 'left' },   
            2: { halign: 'center' },
            3: { halign: 'center' }, 
            4: { halign: 'right' }   
        }
    });

    
    let nombreArchivo = "ReporteVentas";
    if (filtroDesde) nombreArchivo += `_Desde_${new Date(filtroDesde).toLocaleDateString().replace(/\//g, '-')}`;
    if (filtroHasta) nombreArchivo += `_Hasta_${new Date(filtroHasta).toLocaleDateString().replace(/\//g, '-')}`;
    if (filtroClienteId) nombreArchivo += `_Cliente_${filtroClienteId}`;
    doc.save(`${nombreArchivo}.pdf`);
}