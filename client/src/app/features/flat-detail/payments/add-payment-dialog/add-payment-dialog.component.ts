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

@Component({
  selector: 'app-add-payment-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, InputTextModule, InputNumberModule, DatePickerModule, ButtonModule, MessageModule],
  templateUrl: './add-payment-dialog.component.html',
  styleUrl: './add-payment-dialog.component.css',
})
export class AddPaymentDialogComponent {
  flatId = input.required<string>();
  createdById = input.required<string>();
  created = output<void>();

  visible = signal(false);
  loading = signal(false);
  errorMessage = signal('');

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

  open(): void {
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
      flatId: this.flatId(),
      createdById: this.createdById(),
      title: this.form.value.title,
      amount: this.form.value.amount,
      dueDate: dueDate.toISOString(),
    };

    this.paymentService.addPayment(this.flatId(), request).subscribe({
      next: () => {
        this.loading.set(false);
        this.visible.set(false);
        this.created.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to add payment.');
      },
    });
  }
}
