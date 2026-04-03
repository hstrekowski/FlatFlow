import { Component, signal, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { FlatService } from '../../../core/services/flat.service';

@Component({
  selector: 'app-create-flat-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, InputTextModule, ButtonModule, MessageModule],
  templateUrl: './create-flat-dialog.component.html',
  styleUrl: './create-flat-dialog.component.css',
})
export class CreateFlatDialogComponent {
  visible = signal(false);
  loading = signal(false);
  errorMessage = signal('');
  created = output<void>();

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private flatService: FlatService,
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      street: ['', Validators.required],
      city: ['', Validators.required],
      zipCode: ['', Validators.required],
      country: ['', Validators.required],
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

    this.flatService.createFlat(this.form.value).subscribe({
      next: () => {
        this.loading.set(false);
        this.visible.set(false);
        this.created.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to create flat.');
      },
    });
  }
}
