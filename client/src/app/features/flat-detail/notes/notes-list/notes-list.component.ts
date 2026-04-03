import { Component, signal, viewChild, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { NoteService } from '../../../../core/services/note.service';
import { FlatService } from '../../../../core/services/flat.service';
import { AuthService } from '../../../../core/services/auth.service';
import { NoteDto } from '../../../../models/note.model';
import { TenantDto } from '../../../../models/tenant.model';
import { AddNoteDialogComponent } from '../add-note-dialog/add-note-dialog.component';
import { EditNoteDialogComponent } from '../edit-note-dialog/edit-note-dialog.component';

@Component({
  selector: 'app-notes-list',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    ConfirmDialogModule,
    ToastModule,
    AddNoteDialogComponent,
    EditNoteDialogComponent,
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './notes-list.component.html',
  styleUrl: './notes-list.component.css',
})
export class NotesListComponent implements OnInit {
  notes = signal<NoteDto[]>([]);
  tenants = signal<TenantDto[]>([]);
  loading = signal(true);

  flatId = '';
  currentTenantId = '';
  page = 1;
  pageSize = 10;
  totalPages = 1;

  addNoteDialog = viewChild.required(AddNoteDialogComponent);
  editNoteDialog = viewChild.required(EditNoteDialogComponent);

  constructor(
    private route: ActivatedRoute,
    private noteService: NoteService,
    private flatService: FlatService,
    private authService: AuthService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
  ) {}

  ngOnInit(): void {
    this.flatId = this.route.parent!.snapshot.params['id'];
    this.loadFlat();
    this.loadNotes();
  }

  private loadFlat(): void {
    this.flatService.getFlatById(this.flatId).subscribe({
      next: (flat) => {
        this.tenants.set(flat.tenants);
        const email = this.authService.getEmail();
        const currentTenant = flat.tenants.find((t) => t.email === email);
        if (currentTenant) {
          this.currentTenantId = currentTenant.id;
        }
      },
    });
  }

  loadNotes(): void {
    this.loading.set(true);
    this.noteService.getNotesByFlatId(this.flatId, this.page, this.pageSize).subscribe({
      next: (result) => {
        this.notes.set(result.items);
        this.totalPages = result.totalPages;
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  getAuthorName(authorId: string): string {
    const tenant = this.tenants().find((t) => t.id === authorId);
    return tenant ? `${tenant.firstName} ${tenant.lastName}` : 'Unknown';
  }

  openAddNote(): void {
    this.addNoteDialog().open();
  }

  openEditNote(note: NoteDto): void {
    this.editNoteDialog().open(note);
  }

  confirmDeleteNote(note: NoteDto): void {
    this.confirmationService.confirm({
      message: `Delete note "${note.title}"?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.noteService.removeNote(this.flatId, note.id).subscribe({
          next: () => this.loadNotes(),
          error: (err) => this.showError(err),
        });
      },
    });
  }

  prevPage(): void {
    if (this.page > 1) {
      this.page--;
      this.loadNotes();
    }
  }

  nextPage(): void {
    if (this.page < this.totalPages) {
      this.page++;
      this.loadNotes();
    }
  }

  private showError(err: any): void {
    const message = err.error?.message || err.error?.title || 'Something went wrong.';
    this.messageService.add({ severity: 'error', summary: 'Error', detail: message });
  }
}
