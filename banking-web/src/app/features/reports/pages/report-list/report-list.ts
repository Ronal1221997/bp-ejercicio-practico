import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BankApi } from '../../../../core/services/bank-api';
import { Customer } from '../../../../core/models/customer.model';
import { ReportItem } from '../../../../core/models/report.model';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-report-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './report-list.html',
  styleUrl: './report-list.scss'
})
export class ReportList implements OnInit {
  
  private bankService = inject(BankApi);
  private fb = inject(FormBuilder);
  private cd = inject(ChangeDetectorRef);

  customers: Customer[] = [];
  reportData: ReportItem[] = [];
  base64Pdf: string | undefined;

  // Formulario de Filtros
  filterForm: FormGroup = this.fb.group({
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    clienteId: [0, Validators.required] // 0 por defecto = Todos
  });

  ngOnInit() {
    // Cargar clientes para el select
    this.bankService.getCustomers().subscribe(data => this.customers = data);
  }

  isLoading = false;

  generateReport() {
    if (this.filterForm.invalid) return;

    this.isLoading = true;
    this.reportData = [];
    this.base64Pdf = undefined;

    const { startDate, endDate, clienteId } = this.filterForm.value;

    this.bankService.getReports(startDate, endDate, clienteId).subscribe({
      next: (resp) => {
        console.log('Reporte recibido:', resp); // Para depurar

        // Asignamos los datos
        this.reportData = resp.reporte || [];
        this.base64Pdf = resp.archivoPdf;
        
        // 3. Apagamos la carga MANUALMENTE dentro del next
        this.isLoading = false; 

        // 4. ¡FUERZA LA ACTUALIZACIÓN VISUAL!
        this.cd.detectChanges(); 
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        this.cd.detectChanges(); // También actualizamos si hay error
        alert('Error generando el reporte');
      }
    });
  }

  downloadPdf() {
    if (!this.base64Pdf) return;

    // Convertir Base64 a Blob para descargar
    const byteCharacters = atob(this.base64Pdf);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: 'application/pdf' });

    // Crear link invisible y hacer clic
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = `Reporte_${new Date().getTime()}.pdf`;
    link.click();
  }
}