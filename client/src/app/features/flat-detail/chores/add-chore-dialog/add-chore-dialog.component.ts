import { Component, signal, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { SelectModule } from 'primeng/select';
import { ChoreService } from '../../../../core/services/chore.service';
import { ChoreFrequency } from '../../../../models/chore.model';

@Component({
  selector: 'app-add-chore-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, InputTextModule, ButtonModule, MessageModule, SelectModule],
  templateUrl: './add-chore-dialog.component.html',
  styleUrl: './add-chore-dialog.component.css',
})
export class AddChoreDialogComponent {
  flatId = input.required<string>();
  createdById = input.required<string>();
  created = output<void>();

  visible = signal(false);
  loading = signal(false);
  errorMessage = signal('');

  frequencyOptions = [
    { label: 'Once', value: ChoreFrequency.Once },
    { label: 'Daily', value: ChoreFrequency.Daily },
    { label: 'Weekly', value: ChoreFrequency.Weekly },
    { label: 'Monthly', value: ChoreFrequency.Monthly },
  ];

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private choreService: ChoreService,
  ) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      frequency: [ChoreFrequency.Once, Validators.required],
    });
  }

  open(): void {
    this.form.reset({ frequency: ChoreFrequency.Once });
    this.errorMessage.set('');
    this.visible.set(true);
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.errorMessage.set('');

    const request = {
      flatId: this.flatId(),
      createdById: this.createdById(),
      ...this.form.value,
    };

    this.choreService.addChore(this.flatId(), request).subscribe({
      next: () => {
        this.loading.set(false);
        this.visible.set(false);
        this.created.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to add chore.');
      },
    });
  }
}
