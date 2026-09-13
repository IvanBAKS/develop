import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, ...MATERIAL_IMPORTS],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  email = '';
  contrasena = '';
  error = '';
  loading = false;

  onSubmit(): void {
    this.error = '';
    this.loading = true;

    this.auth.login(this.email, this.contrasena).subscribe({
      next: () => {
        this.router.navigate(['/panel']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Email o contraseña incorrectos.';
      }
    });
  }
}
