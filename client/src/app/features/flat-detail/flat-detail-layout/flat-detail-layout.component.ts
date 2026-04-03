import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MenuModule } from 'primeng/menu';
import { DrawerModule } from 'primeng/drawer';
import { ButtonModule } from 'primeng/button';
import { FlatService } from '../../../core/services/flat.service';
import { FlatDetailDto } from '../../../models/flat.model';

@Component({
  selector: 'app-flat-detail-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, MenuModule, DrawerModule, ButtonModule],
  templateUrl: './flat-detail-layout.component.html',
  styleUrl: './flat-detail-layout.component.css',
})
export class FlatDetailLayoutComponent implements OnInit
{
  flat = signal<FlatDetailDto | null>(null);
  sidebarVisible = signal(false);
  flatId = '';

  navItems: { label: string; icon: string; path: string }[] = [];

  constructor(
    private route: ActivatedRoute,
    private flatService: FlatService,
  ) { }

  ngOnInit(): void
  {
    this.flatId = this.route.snapshot.params['id'];
    this.buildNavItems();
    this.loadFlat();
  }

  private buildNavItems(): void
  {
    this.navItems = [
      { label: 'Dashboard', icon: 'pi pi-home', path: `/flats/${this.flatId}` },
      { label: 'Chores', icon: 'pi pi-check-square', path: `/flats/${this.flatId}/chores` },
      { label: 'Payments', icon: 'pi pi-wallet', path: `/flats/${this.flatId}/payments` },
      { label: 'Notes', icon: 'pi pi-file', path: `/flats/${this.flatId}/notes` },
      { label: 'Tenants', icon: 'pi pi-users', path: `/flats/${this.flatId}/tenants` },
      { label: 'Settings', icon: 'pi pi-cog', path: `/flats/${this.flatId}/settings` },
    ];
  }

  private loadFlat(): void
  {
    this.flatService.getFlatById(this.flatId).subscribe({
      next: (flat) => this.flat.set(flat),
    });
  }
}
