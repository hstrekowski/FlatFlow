import { Component, signal, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { switchMap } from 'rxjs';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { FlatService } from '../../../core/services/flat.service';

@Component({
  selector: 'app-join-flat-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, InputTextModule, ButtonModule, MessageModule],
  templateUrl: './join-flat-dialog.component.html',
  styleUrl: './join-flat-dialog.component.css',
})
export class JoinFlatDialogComponent {
  visible = signal(false);
  loading = signal(false);
  errorMessage = signal('');
  flatName = signal('');
  joined = output<void>();

  form: FormGroup;

  private flatId = '';

  constructor(
    private fb: FormBuilder,
    private flatService: FlatService,
  ) {
    this.form = this.fb.group({
      accessCode: ['', Validators.required],
    });
  }

  open(flatId?: string, flatName?: string): void {
    this.flatId = flatId || '';
    this.flatName.set(flatName || '');
    this.form.reset();
    this.errorMessage.set('');
    this.visible.set(true);
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.errorMessage.set('');

    const accessCode = this.form.value.accessCode;

    if (this.flatId) {
      this.flatService.getByAccessCode(accessCode).pipe(
        switchMap((flat) => {
          if (flat.id !== this.flatId) {
            throw new Error('Access code does not match this flat.');
          }
          return this.flatService.joinFlat({ accessCode });
        }),
      ).subscribe({
        next: () => this.onSuccess(),
        error: (err) => this.onError(err),
      });
    } else {
      this.flatService.joinFlat({ accessCode }).subscribe({
        next: () => this.onSuccess(),
        error: (err) => this.onError(err),
      });
    }
  }

  private onSuccess(): void {
    this.loading.set(false);
    this.visible.set(false);
    this.joined.emit();
  }

  private onError(err: unknown): void {
    this.loading.set(false);
    if (err instanceof Error) {
      this.errorMessage.set(err.message);
    } else {
      this.errorMessage.set((err as any).error?.message || 'Failed to join flat.');
    }
  }
}
