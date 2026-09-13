import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { AlbumService, AlbumDetalle, AlbumEquipoConCromos, AlbumCromo } from '../../services/album.service';
import { CromoEditarComponent } from './cromo-editar';

export interface EspecialGrupo {
  tipo: string;
  cromos: AlbumCromo[];
}

@Component({
  selector: 'app-coleccion-detalle',
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatExpansionModule,
    MatProgressSpinnerModule,
    MatDialogModule
  ],
  templateUrl: './coleccion-detalle.html',
  styleUrl: './coleccion-detalle.scss'
})
export class ColeccionDetalleComponent implements OnInit {

  album: AlbumDetalle | null = null;
  cargando = true;
  error = '';

  placeholders = [1, 2, 3, 4];

  especiales: EspecialGrupo[] = [];

  constructor(
    private route: ActivatedRoute,
    private albumService: AlbumService,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef
  ) {}

  editarCromo(cromo: AlbumCromo): void {
    if (!this.album) return;

    const dialogRef = this.dialog.open(CromoEditarComponent, {
      width: '520px',
      data: { cromo, albumId: this.album.id }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && this.album) {
        this.cargarDetalle(this.album.id);
      }
    });
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.error = 'ID de álbum no válido.';
      this.cargando = false;
      return;
    }
    this.cargarDetalle(id);
  }

  trackEquipo(_: number, equipo: any): number {
    return equipo.id;
  }

  trackTipo(_: number, grupo: EspecialGrupo): string {
    return grupo.tipo;
  }

  esBasico(tipo: string): boolean {
    const t = (tipo || '').trim().toLowerCase();
    return t === 'básica' || t === 'basica' || t === 'coloca' || t === 'baja';
  }

  cromosDeEquipo(equipo: AlbumEquipoConCromos): AlbumCromo[] {
    return equipo.cromos
      .filter(c => this.esBasico(c.tipoCromo.nombre))
      .sort((a, b) => this.compararNumero(a, b));
  }

  mostrarEdicion(cromo: AlbumCromo): boolean {
    return cromo.edicion != null && cromo.edicion.id !== 1;
  }

  private compararNumero(a: AlbumCromo, b: AlbumCromo): number {
    return this.numeroValor(a.numero) - this.numeroValor(b.numero);
  }

  private numeroValor(numero: string): number {
    const m = (numero || '').match(/\d+/);
    return m ? parseInt(m[0], 10) : Number.MAX_SAFE_INTEGER;
  }

  private construirEspeciales(data: AlbumDetalle): void {
    const grupos = new Map<string, AlbumCromo[]>();
    for (const equipo of data.equipos) {
      for (const cromo of equipo.cromos) {
        if (this.esBasico(cromo.tipoCromo.nombre)) continue;
        const nombre = cromo.tipoCromo.nombre;
        if (!grupos.has(nombre)) grupos.set(nombre, []);
        grupos.get(nombre)!.push(cromo);
      }
    }
    this.especiales = Array.from(grupos.entries())
      .map(([tipo, cromos]) => ({
        tipo,
        cromos: cromos.sort((a, b) => this.compararNumero(a, b))
      }))
      .sort((a, b) => (a.tipo < b.tipo ? -1 : 1));
  }

  private cargarDetalle(id: number): void {
    this.cargando = true;
    this.albumService.getAlbumConCromos(id).subscribe({
      next: data => {
        this.album = data;
        this.construirEspeciales(data);
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (e) => {
        this.error = 'No se ha podido cargar el álbum.';
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }
}