import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';

@Component({
  selector: 'app-registro',
  imports: [FormsModule, RouterLink, ...MATERIAL_IMPORTS],
  templateUrl: './registro.html',
  styleUrl: './registro.scss'
})
export class RegistroComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  nombre = '';
  email = '';
  contrasena = '';
  confirmar = '';
  error = '';
  loading = false;

  get passwordsMatch(): boolean {
    return this.contrasena === this.confirmar;
  }

  onSubmit(): void {
    this.error = '';

    if (!this.passwordsMatch) {
      this.error = 'Las contraseñas no coinciden.';
      return;
    }

    this.loading = true;

    this.auth.register(this.nombre, this.email, this.contrasena).subscribe({
      next: () => {
        this.router.navigate(['/panel']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Error al registrarse.';
      }
    });
  }
}
