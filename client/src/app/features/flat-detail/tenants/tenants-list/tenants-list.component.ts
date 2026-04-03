import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TenantService } from '../../../../core/services/tenant.service';
import { AuthService } from '../../../../core/services/auth.service';
import { TenantDto } from '../../../../models/tenant.model';

@Component({
  selector: 'app-tenants-list',
  standalone: true,
  imports: [CommonModule, ButtonModule, TagModule, ConfirmDialogModule, ToastModule],
  providers: [ConfirmationService, MessageService],
  templateUrl: './tenants-list.component.html',
  styleUrl: './tenants-list.component.css',
})
export class TenantsListComponent implements OnInit {
  tenants = signal<TenantDto[]>([]);
  loading = signal(true);
  isCurrentUserOwner = false;

  flatId = '';

  constructor(
    private route: ActivatedRoute,
    private tenantService: TenantService,
    private authService: AuthService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
  ) {}

  ngOnInit(): void {
    this.flatId = this.route.parent!.snapshot.params['id'];
    this.loadTenants();
  }

  loadTenants(): void {
    this.loading.set(true);
    this.tenantService.getTenantsByFlatId(this.flatId).subscribe({
      next: (tenants) => {
        this.tenants.set(tenants);
        const email = this.authService.getEmail();
        const currentTenant = tenants.find((t) => t.email === email);
        this.isCurrentUserOwner = currentTenant?.isOwner ?? false;
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  confirmPromote(tenant: TenantDto): void {
    this.confirmationService.confirm({
      message: `Promote ${tenant.firstName} ${tenant.lastName} to owner?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.tenantService.promoteTenant(this.flatId, tenant.id).subscribe({
          next: () => this.loadTenants(),
          error: (err) => this.showError(err),
        });
      },
    });
  }

  confirmRevoke(tenant: TenantDto): void {
    this.confirmationService.confirm({
      message: `Revoke ownership from ${tenant.firstName} ${tenant.lastName}?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.tenantService.revokeOwnership(this.flatId, tenant.id).subscribe({
          next: () => this.loadTenants(),
          error: (err) => this.showError(err),
        });
      },
    });
  }

  confirmRemove(tenant: TenantDto): void {
    this.confirmationService.confirm({
      message: `Remove ${tenant.firstName} ${tenant.lastName} from this flat?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.tenantService.removeTenant(this.flatId, tenant.id).subscribe({
          next: () => this.loadTenants(),
          error: (err) => this.showError(err),
        });
      },
    });
  }

  private showError(err: any): void {
    const message = err.error?.message || err.error?.title || 'Something went wrong.';
    this.messageService.add({ severity: 'error', summary: 'Error', detail: message });
  }
}
