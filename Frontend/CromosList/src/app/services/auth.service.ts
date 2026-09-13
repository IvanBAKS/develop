import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';

export interface AuthResponse {
  token: string;
  nombre: string;
  email: string;
  esAdmin: boolean;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = '/api/auth';

  private userSignal = signal<{ nombre: string; email: string; esAdmin: boolean } | null>(this.loadUser());

  readonly user = this.userSignal.asReadonly();
  readonly isLoggedIn = computed(() => !!this.userSignal());
  readonly isAdmin = computed(() => this.userSignal()?.esAdmin ?? false);

  constructor(private http: HttpClient) {}

  login(email: string, contrasena: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { email, contrasena }).pipe(
      tap((res) => this.storeSession(res))
    );
  }

  register(nombre: string, email: string, contrasena: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, { nombre, email, contrasena }).pipe(
      tap((res) => this.storeSession(res))
    );
  }

  logout(): void {
    sessionStorage.removeItem('auth_token');
    sessionStorage.removeItem('auth_user');
    this.userSignal.set(null);
  }

  getToken(): string | null {
    return sessionStorage.getItem('auth_token');
  }

  private storeSession(res: AuthResponse): void {
    sessionStorage.setItem('auth_token', res.token);
    sessionStorage.setItem('auth_user', JSON.stringify({ nombre: res.nombre, email: res.email, esAdmin: res.esAdmin }));
    this.userSignal.set({ nombre: res.nombre, email: res.email, esAdmin: res.esAdmin });
  }

  private loadUser(): { nombre: string; email: string; esAdmin: boolean } | null {
    const raw = sessionStorage.getItem('auth_user');
    if (!raw) return null;
    const parsed = JSON.parse(raw) as { nombre: string; email: string; esAdmin?: boolean };
    return { nombre: parsed.nombre, email: parsed.email, esAdmin: parsed.esAdmin ?? false };
  }
}
