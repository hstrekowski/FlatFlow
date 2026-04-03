import { Component, signal, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { NoteService } from '../../../../core/services/note.service';
import { NoteDto } from '../../../../models/note.model';

@Component({
  selector: 'app-edit-note-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DialogModule, InputTextModule, TextareaModule, ButtonModule, MessageModule],
  templateUrl: './edit-note-dialog.component.html',
  styleUrl: './edit-note-dialog.component.css',
})
export class EditNoteDialogComponent {
  flatId = input.required<string>();
  updated = output<void>();

  visible = signal(false);
  loading = signal(false);
  errorMessage = signal('');

  private noteId = '';

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

  open(note: NoteDto): void {
    this.noteId = note.id;
    this.form.patchValue({
      title: note.title,
      content: note.content,
    });
    this.errorMessage.set('');
    this.visible.set(true);
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.errorMessage.set('');

    const request = {
      noteId: this.noteId,
      ...this.form.value,
    };

    this.noteService.updateNote(this.flatId(), this.noteId, request).subscribe({
      next: () => {
        this.loading.set(false);
        this.visible.set(false);
        this.updated.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to update note.');
      },
    });
  }
}
