import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { FlatService } from '../../../core/services/flat.service';
import { FlatDetailDto } from '../../../models/flat.model';

@Component({
  selector: 'app-flat-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputTextModule, ButtonModule, MessageModule, ConfirmDialogModule, ToastModule],
  providers: [ConfirmationService, MessageService],
  templateUrl: './flat-settings.component.html',
  styleUrl: './flat-settings.component.css',
})
export class FlatSettingsComponent implements OnInit {
  flat = signal<FlatDetailDto | null>(null);
  loading = signal(true);
  saving = signal(false);
  refreshing = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  accessCode = signal('');

  flatId = '';
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private flatService: FlatService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      street: ['', Validators.required],
      city: ['', Validators.required],
      zipCode: ['', Validators.required],
      country: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.flatId = this.route.parent!.snapshot.params['id'];
    this.loadFlat();
  }

  private loadFlat(): void {
    this.flatService.getFlatById(this.flatId).subscribe({
      next: (flat) => {
        this.flat.set(flat);
        this.accessCode.set(flat.accessCode);
        this.form.patchValue({
          name: flat.name,
          street: flat.street,
          city: flat.city,
          zipCode: flat.zipCode,
          country: flat.country,
        });
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  onSave(): void {
    if (this.form.invalid) return;

    this.saving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const request = {
      flatId: this.flatId,
      ...this.form.value,
    };

    this.flatService.updateFlat(this.flatId, request).subscribe({
      next: () => {
        this.saving.set(false);
        this.messageService.add({ severity: 'success', summary: 'Saved', detail: 'Flat updated successfully.' });
      },
      error: (err) => {
        this.saving.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to update flat.' });
      },
    });
  }

  onRefreshAccessCode(): void {
    this.refreshing.set(true);
    this.flatService.refreshAccessCode(this.flatId).subscribe({
      next: () => {
        this.refreshing.set(false);
        this.flatService.getFlatById(this.flatId).subscribe({
          next: (flat) => {
            this.accessCode.set(flat.accessCode);
            this.messageService.add({ severity: 'success', summary: 'Refreshed', detail: 'Access code has been refreshed.' });
          },
        });
      },
      error: (err) => {
        this.refreshing.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to refresh access code.' });
      },
    });
  }

  confirmDelete(): void {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete this flat? This action cannot be undone.',
      header: 'Delete Flat',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.flatService.deleteFlat(this.flatId).subscribe({
          next: () => this.router.navigate(['/flats']),
          error: (err) => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Failed to delete flat.' });
          },
        });
      },
    });
  }
}
