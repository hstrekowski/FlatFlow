import { Component, signal, viewChild, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { PaymentService } from '../../../../core/services/payment.service';
import { FlatService } from '../../../../core/services/flat.service';
import { AuthService } from '../../../../core/services/auth.service';
import { PaymentDetailDto, PaymentShareDto, PaymentShareStatus } from '../../../../models/payment.model';
import { TenantDto } from '../../../../models/tenant.model';
import { AddPaymentDialogComponent } from '../add-payment-dialog/add-payment-dialog.component';
import { EditPaymentDialogComponent } from '../edit-payment-dialog/edit-payment-dialog.component';
import { AddShareDialogComponent } from '../add-share-dialog/add-share-dialog.component';

@Component({
  selector: 'app-payments-list',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    TagModule,
    ConfirmDialogModule,
    ToastModule,
    AddPaymentDialogComponent,
    EditPaymentDialogComponent,
    AddShareDialogComponent,
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './payments-list.component.html',
  styleUrl: './payments-list.component.css',
})
export class PaymentsListComponent implements OnInit {
  payments = signal<PaymentDetailDto[]>([]);
  tenants = signal<TenantDto[]>([]);
  loading = signal(true);

  flatId = '';
  currentTenantId = '';
  page = 1;
  pageSize = 10;
  totalPages = 1;

  addPaymentDialog = viewChild.required(AddPaymentDialogComponent);
  editPaymentDialog = viewChild.required(EditPaymentDialogComponent);
  addShareDialog = viewChild.required(AddShareDialogComponent);

  constructor(
    private route: ActivatedRoute,
    private paymentService: PaymentService,
    private flatService: FlatService,
    private authService: AuthService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
  ) {}

  ngOnInit(): void {
    this.flatId = this.route.parent!.snapshot.params['id'];
    this.loadFlat();
    this.loadPayments();
  }

  private loadFlat(): void {
    this.flatService.getFlatById(this.flatId).subscribe({
      next: (flat) => {
        this.tenants.set(flat.tenants);
        const email = this.authService.getEmail();
        const currentTenant = flat.tenants.find((t) => t.email === email);
        if (currentTenant) {
          this.currentTenantId = currentTenant.id;
        }
      },
    });
  }

  loadPayments(): void {
    this.loading.set(true);
    this.paymentService.getPaymentsByFlatId(this.flatId, this.page, this.pageSize).subscribe({
      next: (result) => {
        this.totalPages = result.totalPages;
        if (result.items.length === 0) {
          this.payments.set([]);
          this.loading.set(false);
          return;
        }
        const detailRequests = result.items.map((p) => this.paymentService.getPaymentById(this.flatId, p.id));
        forkJoin(detailRequests).subscribe({
          next: (details) => {
            this.payments.set(details);
            this.loading.set(false);
          },
          error: () => this.loading.set(false),
        });
      },
      error: () => this.loading.set(false),
    });
  }

  getTenantName(tenantId: string): string {
    const tenant = this.tenants().find((t) => t.id === tenantId);
    return tenant ? `${tenant.firstName} ${tenant.lastName}` : 'Unknown';
  }

  getShareStatusLabel(status: PaymentShareStatus): string {
    switch (status) {
      case PaymentShareStatus.New: return 'New';
      case PaymentShareStatus.Partial: return 'Partial';
      case PaymentShareStatus.Paid: return 'Paid';
      default: return 'Unknown';
    }
  }

  getShareStatusSeverity(status: PaymentShareStatus): 'info' | 'warn' | 'success' {
    switch (status) {
      case PaymentShareStatus.New: return 'info';
      case PaymentShareStatus.Partial: return 'warn';
      case PaymentShareStatus.Paid: return 'success';
      default: return 'info';
    }
  }

  openAddPayment(): void {
    this.addPaymentDialog().open();
  }

  openEditPayment(payment: PaymentDetailDto): void {
    this.editPaymentDialog().open(payment);
  }

  openAddShare(paymentId: string): void {
    this.addShareDialog().open(paymentId);
  }

  confirmDeletePayment(payment: PaymentDetailDto): void {
    this.confirmationService.confirm({
      message: `Delete payment "${payment.title}"?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.paymentService.removePayment(this.flatId, payment.id).subscribe({
          next: () => this.loadPayments(),
          error: (err) => this.showError(err),
        });
      },
    });
  }

  markAsPaid(paymentId: string, shareId: string): void {
    this.paymentService.markShareAsPaid(this.flatId, paymentId, shareId).subscribe({
      next: () => this.reloadPaymentDetail(paymentId),
      error: (err) => this.showError(err),
    });
  }

  markAsPartial(paymentId: string, shareId: string): void {
    this.paymentService.markShareAsPartial(this.flatId, paymentId, shareId).subscribe({
      next: () => this.reloadPaymentDetail(paymentId),
      error: (err) => this.showError(err),
    });
  }

  confirmDeleteShare(paymentId: string, share: PaymentShareDto): void {
    this.confirmationService.confirm({
      message: 'Remove this share?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.paymentService.removeShare(this.flatId, paymentId, share.id).subscribe({
          next: () => this.reloadPaymentDetail(paymentId),
          error: (err) => this.showError(err),
        });
      },
    });
  }

  prevPage(): void {
    if (this.page > 1) {
      this.page--;
      this.loadPayments();
    }
  }

  nextPage(): void {
    if (this.page < this.totalPages) {
      this.page++;
      this.loadPayments();
    }
  }

  private reloadPaymentDetail(paymentId: string): void {
    this.paymentService.getPaymentById(this.flatId, paymentId).subscribe({
      next: (detail) => {
        const updated = this.payments().map((p) => (p.id === paymentId ? detail : p));
        this.payments.set(updated);
      },
    });
  }

  private showError(err: any): void {
    const message = err.error?.message || err.error?.title || 'Something went wrong.';
    this.messageService.add({ severity: 'error', summary: 'Error', detail: message });
  }
}
