import { Component, signal, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { NoteService } from '../../../../core/services/note.service';

@Component({
  selector: 'app-add-note-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, InputTextModule, TextareaModule, ButtonModule, MessageModule],
  templateUrl: './add-note-dialog.component.html',
  styleUrl: './add-note-dialog.component.css',
})
export class AddNoteDialogComponent {
  flatId = input.required<string>();
  authorId = input.required<string>();
  created = output<void>();

  visible = signal(false);
  loading = signal(false);
  errorMessage = signal('');

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private noteService: NoteService,
  ) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      content: ['', Validators.required],
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

    const request = {
      flatId: this.flatId(),
      authorId: this.authorId(),
      ...this.form.value,
    };

    this.noteService.addNote(this.flatId(), request).subscribe({
      next: () => {
        this.loading.set(false);
        this.visible.set(false);
        this.created.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to add note.');
      },
    });
  }
}
