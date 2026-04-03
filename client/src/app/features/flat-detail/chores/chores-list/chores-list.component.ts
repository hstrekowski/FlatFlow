import { Component, signal, viewChild, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { CardModule } from 'primeng/card';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ChoreService } from '../../../../core/services/chore.service';
import { FlatService } from '../../../../core/services/flat.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ChoreDetailDto, ChoreAssignmentDto, ChoreFrequency } from '../../../../models/chore.model';
import { TenantDto } from '../../../../models/tenant.model';
import { AddChoreDialogComponent } from '../add-chore-dialog/add-chore-dialog.component';
import { EditChoreDialogComponent } from '../edit-chore-dialog/edit-chore-dialog.component';
import { AddAssignmentDialogComponent } from '../add-assignment-dialog/add-assignment-dialog.component';

@Component({
  selector: 'app-chores-list',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    TagModule,
    CardModule,
    ConfirmDialogModule,
    ToastModule,
    AddChoreDialogComponent,
    EditChoreDialogComponent,
    AddAssignmentDialogComponent,
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './chores-list.component.html',
  styleUrl: './chores-list.component.css',
})
export class ChoresListComponent implements OnInit {
  chores = signal<ChoreDetailDto[]>([]);
  tenants = signal<TenantDto[]>([]);
  loading = signal(true);

  flatId = '';
  currentTenantId = '';

  addChoreDialog = viewChild.required(AddChoreDialogComponent);
  editChoreDialog = viewChild.required(EditChoreDialogComponent);
  addAssignmentDialog = viewChild.required(AddAssignmentDialogComponent);

  private readonly frequencyLabels: Record<number, string> = {
    [ChoreFrequency.Once]: 'Once',
    [ChoreFrequency.Daily]: 'Daily',
    [ChoreFrequency.Weekly]: 'Weekly',
    [ChoreFrequency.Monthly]: 'Monthly',
  };

  constructor(
    private route: ActivatedRoute,
    private choreService: ChoreService,
    private flatService: FlatService,
    private authService: AuthService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
  ) {}

  ngOnInit(): void {
    this.flatId = this.route.parent!.snapshot.params['id'];
    this.loadFlat();
    this.loadChores();
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

  loadChores(): void {
    this.loading.set(true);
    this.choreService.getChoresByFlatId(this.flatId).subscribe({
      next: (chores) => {
        if (chores.length === 0) {
          this.chores.set([]);
          this.loading.set(false);
          return;
        }
        const detailRequests = chores.map((c) => this.choreService.getChoreById(this.flatId, c.id));
        forkJoin(detailRequests).subscribe({
          next: (details) => {
            this.chores.set(details);
            this.loading.set(false);
          },
          error: () => this.loading.set(false),
        });
      },
      error: () => this.loading.set(false),
    });
  }

  getFrequencyLabel(frequency: ChoreFrequency): string {
    return this.frequencyLabels[frequency] || 'Unknown';
  }

  getTenantName(tenantId: string): string {
    const tenant = this.tenants().find((t) => t.id === tenantId);
    return tenant ? `${tenant.firstName} ${tenant.lastName}` : 'Unknown';
  }

  openAddChore(): void {
    this.addChoreDialog().open();
  }

  openEditChore(chore: ChoreDetailDto): void {
    this.editChoreDialog().open(chore);
  }

  openAddAssignment(choreId: string): void {
    this.addAssignmentDialog().open(choreId);
  }

  confirmDeleteChore(chore: ChoreDetailDto): void {
    this.confirmationService.confirm({
      message: `Delete chore "${chore.title}"?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.choreService.removeChore(this.flatId, chore.id).subscribe({
          next: () => this.loadChores(),
          error: (err) => this.showError(err),
        });
      },
    });
  }

  completeAssignment(choreId: string, assignmentId: string): void {
    this.choreService.completeAssignment(this.flatId, choreId, assignmentId).subscribe({
      next: () => this.loadChoreDetail(choreId),
      error: (err) => this.showError(err),
    });
  }

  reopenAssignment(choreId: string, assignmentId: string): void {
    this.choreService.reopenAssignment(this.flatId, choreId, assignmentId).subscribe({
      next: () => this.loadChoreDetail(choreId),
      error: (err) => this.showError(err),
    });
  }

  confirmDeleteAssignment(choreId: string, assignment: ChoreAssignmentDto): void {
    this.confirmationService.confirm({
      message: `Remove this assignment?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.choreService.removeAssignment(this.flatId, choreId, assignment.id).subscribe({
          next: () => this.loadChoreDetail(choreId),
          error: (err) => this.showError(err),
        });
      },
    });
  }

  private showError(err: any): void {
    const message = err.error?.message || err.error?.title || 'Something went wrong.';
    this.messageService.add({ severity: 'error', summary: 'Error', detail: message });
  }

  private loadChoreDetail(choreId: string): void {
    this.choreService.getChoreById(this.flatId, choreId).subscribe({
      next: (detail) => {
        const updated = this.chores().map((c) => (c.id === choreId ? detail : c));
        this.chores.set(updated);
      },
    });
  }

}
