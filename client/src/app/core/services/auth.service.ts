import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  ProfileDto,
} from '../../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'flatflow_token';
  private loggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  isLoggedIn$ = this.loggedIn$.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router,
  ) {}

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(tap((res) => this.setToken(res.token)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/register`, request)
      .pipe(tap((res) => this.setToken(res.token)));
  }

  getProfile(): Observable<ProfileDto> {
    return this.http.get<ProfileDto>(`${environment.apiUrl}/auth/me`);
  }

  updateProfile(data: { firstName: string; lastName: string; email: string }): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/auth/me`, data);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.loggedIn$.next(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    this.loggedIn$.next(true);
  }

  private hasToken(): boolean {
    return !!this.getToken();
  }
}
