import { Component, signal, viewChild, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { FlatService } from '../../../core/services/flat.service';
import { FlatDto } from '../../../models/flat.model';
import { PaginatedResult } from '../../../models/paginated-result.model';
import { CreateFlatDialogComponent } from '../create-flat-dialog/create-flat-dialog.component';
import { JoinFlatDialogComponent } from '../join-flat-dialog/join-flat-dialog.component';

@Component({
  selector: 'app-flats-list',
  standalone: true,
  imports: [CommonModule, CardModule, ButtonModule, PaginatorModule, CreateFlatDialogComponent, JoinFlatDialogComponent],
  templateUrl: './flats-list.component.html',
  styleUrl: './flats-list.component.css',
})
export class FlatsListComponent implements OnInit {
  myFlats = signal<FlatDto[]>([]);
  allFlats = signal<FlatDto[]>([]);
  loading = signal(true);

  page = 1;
  pageSize = 12;
  totalCount = 0;

  createDialog = viewChild.required(CreateFlatDialogComponent);
  joinDialog = viewChild.required(JoinFlatDialogComponent);

  constructor(
    private flatService: FlatService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loadFlats();
  }

  loadFlats(): void {
    this.loading.set(true);
    this.flatService.getMyFlats().subscribe({
      next: (flats) => this.myFlats.set(flats),
    });
    this.flatService.getAllFlats(this.page, this.pageSize).subscribe({
      next: (result: PaginatedResult<FlatDto>) => {
        this.allFlats.set(result.items);
        this.totalCount = result.totalCount;
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  onPageChange(event: PaginatorState): void {
    this.page = Math.floor((event.first ?? 0) / (event.rows ?? this.pageSize)) + 1;
    this.pageSize = event.rows ?? this.pageSize;
    this.loadAllFlats();
  }

  private loadAllFlats(): void {
    this.flatService.getAllFlats(this.page, this.pageSize).subscribe({
      next: (result: PaginatedResult<FlatDto>) => {
        this.allFlats.set(result.items);
        this.totalCount = result.totalCount;
      },
    });
  }

  openCreateDialog(): void {
    this.createDialog().open();
  }

  openJoinDialog(flat: FlatDto): void {
    this.joinDialog().open(flat.id, flat.name);
  }

  openGenericJoinDialog(): void {
    this.joinDialog().open();
  }

  onFlatCreated(): void {
    this.loadFlats();
  }

  onFlatJoined(): void {
    this.loadFlats();
  }

  goToFlat(flatId: string): void {
    this.router.navigate(['/flats', flatId]);
  }

  isMyFlat(flat: FlatDto): boolean {
    return this.myFlats().some((f) => f.id === flat.id);
  }
}
