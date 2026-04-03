import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, ToolbarModule, ButtonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  firstName: string;

  constructor(private authService: AuthService) {
    this.firstName = this.authService.getFirstName();
  }

  onLogout(): void {
    this.authService.logout();
  }
}
