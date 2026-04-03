import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { CardModule } from 'primeng/card';
import { FlatService } from '../../../core/services/flat.service';
import { FlatDetailDto } from '../../../models/flat.model';

@Component({
  selector: 'app-flat-dashboard',
  standalone: true,
  imports: [CommonModule, CardModule],
  templateUrl: './flat-dashboard.component.html',
  styleUrl: './flat-dashboard.component.css',
})
export class FlatDashboardComponent implements OnInit {
  flat = signal<FlatDetailDto | null>(null);
  loading = signal(true);

  constructor(
    private route: ActivatedRoute,
    private flatService: FlatService,
  ) {}

  ngOnInit(): void {
    const flatId = this.route.parent!.snapshot.params['id'];
    this.flatService.getFlatById(flatId).subscribe({
      next: (flat) => {
        this.flat.set(flat);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  get choreCount(): number {
    return this.flat()?.chores?.length ?? 0;
  }

  get paymentCount(): number {
    return this.flat()?.payments?.length ?? 0;
  }

  get noteCount(): number {
    return this.flat()?.notes?.length ?? 0;
  }

  get tenantCount(): number {
    return this.flat()?.tenants?.length ?? 0;
  }
}
