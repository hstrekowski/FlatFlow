import { Component, signal, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { SelectModule } from 'primeng/select';
import { ChoreService } from '../../../../core/services/chore.service';
import { ChoreFrequency, ChoreDetailDto } from '../../../../models/chore.model';

@Component({
  selector: 'app-edit-chore-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, InputTextModule, ButtonModule, MessageModule, SelectModule],
  templateUrl: './edit-chore-dialog.component.html',
  styleUrl: './edit-chore-dialog.component.css',
})
export class EditChoreDialogComponent {
  flatId = input.required<string>();
  updated = output<void>();

  visible = signal(false);
  loading = signal(false);
  errorMessage = signal('');

  private choreId = '';

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

  open(chore: ChoreDetailDto): void {
    this.choreId = chore.id;
    this.form.patchValue({
      title: chore.title,
      description: chore.description,
      frequency: chore.frequency,
    });
    this.errorMessage.set('');
    this.visible.set(true);
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.errorMessage.set('');

    const request = {
      choreId: this.choreId,
      ...this.form.value,
    };

    this.choreService.updateChore(this.flatId(), this.choreId, request).subscribe({
      next: () => {
        this.loading.set(false);
        this.visible.set(false);
        this.updated.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to update chore.');
      },
    });
  }
}
