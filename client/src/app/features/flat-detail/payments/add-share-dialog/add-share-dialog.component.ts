import { Component, signal, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { SelectModule } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';
import { PaymentService } from '../../../../core/services/payment.service';
import { TenantDto } from '../../../../models/tenant.model';

@Component({
  selector: 'app-add-share-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, ButtonModule, MessageModule, SelectModule, InputNumberModule],
  templateUrl: './add-share-dialog.component.html',
  styleUrl: './add-share-dialog.component.css',
})
export class AddShareDialogComponent {
  flatId = input.required<string>();
  tenants = input.required<TenantDto[]>();
  created = output<void>();

  visible = signal(false);
  loading = signal(false);
  errorMessage = signal('');

  private paymentId = '';

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private paymentService: PaymentService,
  ) {
    this.form = this.fb.group({
      tenantId: ['', Validators.required],
      shareAmount: [null, [Validators.required, Validators.min(0.01)]],
    });
  }

  open(paymentId: string): void {
    this.paymentId = paymentId;
    this.form.reset();
    this.errorMessage.set('');
    this.visible.set(true);
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.errorMessage.set('');

    const request = {
      paymentId: this.paymentId,
      tenantId: this.form.value.tenantId,
      shareAmount: this.form.value.shareAmount,
    };

    this.paymentService.addShare(this.flatId(), this.paymentId, request).subscribe({
      next: () => {
        this.loading.set(false);
        this.visible.set(false);
        this.created.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to add share.');
      },
    });
  }
}
