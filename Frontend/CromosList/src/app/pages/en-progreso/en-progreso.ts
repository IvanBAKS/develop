import { Component } from '@angular/core';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';

@Component({
  selector: 'app-en-progreso',
  imports: [...MATERIAL_IMPORTS],
  template: `
    <div class="progreso-container">
      <mat-icon class="progreso-icon">construction</mat-icon>
      <h1>En progreso</h1>
      <p>Esta sección está en desarrollo. Vuelve pronto.</p>
    </div>
  `,
  styles: `
    .progreso-container {
      min-height: calc(100vh - 64px);
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 12px;
      padding: 40px 24px;
      background: var(--app-bg);
      color: var(--app-text-muted);
      text-align: center;
    }

    .progreso-icon {
      font-size: 56px;
      width: 56px;
      height: 56px;
      color: var(--app-primary);
    }

    h1 {
      margin: 0;
      font-size: 28px;
      color: var(--app-text);
    }

    p {
      margin: 0;
      font-size: 15px;
    }
  `
})
export class EnProgresoComponent {}
