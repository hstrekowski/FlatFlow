import { Component, signal, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { ChoreService } from '../../../../core/services/chore.service';
import { TenantDto } from '../../../../models/tenant.model';

@Component({
  selector: 'app-add-assignment-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, ButtonModule, MessageModule, SelectModule, DatePickerModule],
  templateUrl: './add-assignment-dialog.component.html',
  styleUrl: './add-assignment-dialog.component.css',
})
export class AddAssignmentDialogComponent {
  flatId = input.required<string>();
  tenants = input.required<TenantDto[]>();
  created = output<void>();

  visible = signal(false);
  loading = signal(false);
  errorMessage = signal('');

  private choreId = '';

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private choreService: ChoreService,
  ) {
    this.form = this.fb.group({
      tenantId: ['', Validators.required],
      dueDate: [null, Validators.required],
    });
  }

  open(choreId: string): void {
    this.choreId = choreId;
    this.form.reset();
    this.errorMessage.set('');
    this.visible.set(true);
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.errorMessage.set('');

    const dueDate: Date = this.form.value.dueDate;

    const request = {
      choreId: this.choreId,
      tenantId: this.form.value.tenantId,
      dueDate: dueDate.toISOString(),
    };

    this.choreService.addAssignment(this.flatId(), this.choreId, request).subscribe({
      next: () => {
        this.loading.set(false);
        this.visible.set(false);
        this.created.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to add assignment.');
      },
    });
  }
}
