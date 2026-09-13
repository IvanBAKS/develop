import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AlbumService, Album } from '../../services/album.service';
import { UsuarioAlbumService, ColeccionSuscripcion } from '../../services/usuario-album.service';
import { AuthService } from '../../services/auth.service';

export interface ColeccionItem {
  albumId: number;
  nombre: string;
  temporada: string;
  editorial: { id: number; nombre: string };
  totalCromos: number;
  suscrito: boolean;
  tenidos: number;
  faltan: number;
  completado: boolean;
}

@Component({
  selector: 'app-mis-colecciones',
  imports: [
    CommonModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './mis-colecciones.html',
  styleUrl: './mis-colecciones.scss'
})
export class MisColeccionesComponent implements OnInit {

  colecciones: ColeccionItem[] = [];
  cargando = true;
  cambiando = false;
  error = '';

  private miId: number | null = null;

  constructor(
    private albumService: AlbumService,
    private usuarioAlbumService: UsuarioAlbumService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.miId = this.authService.getMiId();
    if (!this.miId) {
      this.error = 'No se ha podido identificar al usuario.';
      this.cargando = false;
      return;
    }
    this.cargar();
  }

  get tieneSuscripciones(): boolean {
    return this.colecciones.some(c => c.suscrito);
  }

  trackAlbum(_: number, item: ColeccionItem): number {
    return item.albumId;
  }

  progreso(item: ColeccionItem): number {
    if (!item.totalCromos) return 0;
    return Math.round((item.tenidos / item.totalCromos) * 100);
  }

  private cargar(): void {
    this.cargando = true;
    this.error = '';
    const usuarioId = this.miId!;

    this.albumService.getAlbumes().subscribe({
      next: albumes => {
        this.usuarioAlbumService.listarSuscripciones(usuarioId).subscribe({
          next: suscripciones => {
            this.colecciones = this.combinar(albumes, suscripciones);
            this.cargando = false;
            this.cdr.detectChanges();
          },
          error: () => {
            this.cargando = false;
            this.error = 'No se han podido cargar tus colecciones.';
            this.cdr.detectChanges();
          }
        });
      },
      error: () => {
        this.cargando = false;
        this.error = 'No se han podido cargar las colecciones.';
        this.cdr.detectChanges();
      }
    });
  }

  private combinar(albumes: Album[], suscripciones: ColeccionSuscripcion[]): ColeccionItem[] {
    const suscritos = new Map(suscripciones.map(s => [s.albumId, s]));

    return albumes.map(a => {
      const s = suscritos.get(a.id);
      return {
        albumId: a.id,
        nombre: a.nombre,
        temporada: a.temporada,
        editorial: a.editorial,
        totalCromos: s?.totalCromos ?? a.contadorCromos,
        suscrito: !!s,
        tenidos: s?.tenidos ?? 0,
        faltan: s?.faltan ?? a.contadorCromos,
        completado: s?.completado ?? false
      };
    }).sort((a, b) => (a.suscrito === b.suscrito ? a.nombre.localeCompare(b.nombre) : a.suscrito ? -1 : 1));
  }

  apuntarse(item: ColeccionItem): void {
    if (!this.miId || this.cambiando) return;
    this.cambiando = true;
    this.error = '';

    this.usuarioAlbumService.suscribir({ usuarioId: this.miId, albumId: item.albumId }).subscribe({
      next: () => {
        this.cambiando = false;
        this.cargar();
      },
      error: () => {
        this.cambiando = false;
        this.error = 'No se ha podido apuntar a la colección.';
        this.cdr.detectChanges();
      }
    });
  }

  desapuntarse(item: ColeccionItem): void {
    if (!this.miId || this.cambiando) return;
    this.cambiando = true;
    this.error = '';

    this.usuarioAlbumService.desuscribir(this.miId, item.albumId).subscribe({
      next: () => {
        this.cambiando = false;
        this.cargar();
      },
      error: () => {
        this.cambiando = false;
        this.error = 'No se ha podido desapuntar de la colección.';
        this.cdr.detectChanges();
      }
    });
  }
}