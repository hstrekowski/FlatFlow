import { Component, signal, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { PaymentService } from '../../../../core/services/payment.service';
import { PaymentDetailDto } from '../../../../models/payment.model';

@Component({
  selector: 'app-edit-payment-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, InputTextModule, InputNumberModule, DatePickerModule, ButtonModule, MessageModule],
  templateUrl: './edit-payment-dialog.component.html',
  styleUrl: './edit-payment-dialog.component.css',
})
export class EditPaymentDialogComponent {
  flatId = input.required<string>();
  updated = output<void>();

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
      title: ['', Validators.required],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      dueDate: [null, Validators.required],
    });
  }

  open(payment: PaymentDetailDto): void {
    this.paymentId = payment.id;
    this.form.patchValue({
      title: payment.title,
      amount: payment.amount,
      dueDate: new Date(payment.dueDate),
    });
    this.errorMessage.set('');
    this.visible.set(true);
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.errorMessage.set('');

    const dueDate: Date = this.form.value.dueDate;
    const request = {
      paymentId: this.paymentId,
      title: this.form.value.title,
      amount: this.form.value.amount,
      dueDate: dueDate.toISOString(),
    };

    this.paymentService.updatePayment(this.flatId(), this.paymentId, request).subscribe({
      next: () => {
        this.loading.set(false);
        this.visible.set(false);
        this.updated.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to update payment.');
      },
    });
  }
}
