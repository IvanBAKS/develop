import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { AlbumService, AlbumDetalle, AlbumEquipoConCromos, AlbumCromo } from '../../services/album.service';
import { UsuarioAlbumService, ColeccionProgreso } from '../../services/usuario-album.service';
import { UsuarioCromoService } from '../../services/usuario-cromo.service';
import { AuthService } from '../../services/auth.service';
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
    MatProgressBarModule,
    MatButtonModule,
    MatCheckboxModule,
    MatDialogModule
  ],
  templateUrl: './coleccion-detalle.html',
  styleUrl: './coleccion-detalle.scss'
})
export class ColeccionDetalleComponent implements OnInit {

  album: AlbumDetalle | null = null;
  cargando = true;
  error = '';

  progreso: ColeccionProgreso | null = null;
  marcando = false;
  soloFaltan = false;

  placeholders = [1, 2, 3, 4];

  especiales: EspecialGrupo[] = [];

  private miId: number | null = null;
  private tenidosIds = new Set<number>();

  constructor(
    private route: ActivatedRoute,
    private albumService: AlbumService,
    private usuarioAlbumService: UsuarioAlbumService,
    private usuarioCromoService: UsuarioCromoService,
    private authService: AuthService,
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
    this.miId = this.authService.getMiId();
    this.cargarDetalle(id);
  }

  get suscrito(): boolean {
    return this.progreso?.suscrito ?? false;
  }

  get esAdmin(): boolean {
    return this.authService.isAdmin();
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

  cromosVisibles(equipo: AlbumEquipoConCromos): AlbumCromo[] {
    const cromos = this.cromosDeEquipo(equipo);
    return this.soloFaltan
      ? cromos.filter(c => !this.loTiene(c))
      : cromos;
  }

  get especialesVisibles(): EspecialGrupo[] {
    if (!this.soloFaltan) return this.especiales;
    return this.especiales
      .map(grupo => ({
        tipo: grupo.tipo,
        cromos: grupo.cromos.filter(c => !this.loTiene(c))
      }))
      .filter(grupo => grupo.cromos.length > 0);
  }

  cambiarFiltro(checked: boolean): void {
    this.soloFaltan = checked;
    this.cdr.detectChanges();
  }

  mostrarEdicion(cromo: AlbumCromo): boolean {
    return cromo.edicion != null && cromo.edicion.id !== 1;
  }

  puedeMarcar(): boolean {
    return this.suscrito && !this.marcando;
  }

  loTiene(cromo: AlbumCromo): boolean {
    return this.tenidosIds.has(cromo.id);
  }

  progresoPct(): number {
    if (!this.progreso || !this.progreso.totalCromos) return 0;
    return Math.round((this.progreso.tenidos / this.progreso.totalCromos) * 100);
  }

  alternarCromo(cromo: AlbumCromo): void {
    if (!this.miId || !this.suscrito || this.marcando) return;

    const tiene = this.loTiene(cromo);
    this.marcando = true;

    const operacion = tiene
      ? this.usuarioCromoService.desmarcarCromo(this.miId, cromo.id)
      : this.usuarioCromoService.marcarCromo(this.miId, cromo.id);

    operacion.subscribe({
      next: () => {
        this.marcando = false;
        this.cargarProgreso();
      },
      error: () => {
        this.marcando = false;
        this.error = 'No se ha podido actualizar el cromo.';
        this.cdr.detectChanges();
      }
    });
  }

  apuntarse(): void {
    if (!this.miId || !this.album || this.marcando) return;
    this.marcando = true;

    this.usuarioAlbumService.suscribir({ usuarioId: this.miId, albumId: this.album.id }).subscribe({
      next: () => {
        this.marcando = false;
        this.cargarProgreso();
      },
      error: () => {
        this.marcando = false;
        this.error = 'No se ha podido apuntar a la colección.';
        this.cdr.detectChanges();
      }
    });
  }

  private compararNumero(a: AlbumCromo, b: AlbumCromo): number {
    return this.numeroValor(a.numero) - this.numeroValor(b.numero);
  }

  private numeroValor(numero: string): number {
    const m = (numero || '').match(/\d+/);
    return m ? parseInt(m[0], 10) : Number.MAX_SAFE_INTEGER;
  }

  private construirEspeciales(data: AlbumDetalle): void {
    const grupos = new Map<number, AlbumCromo[]>();
    for (const equipo of data.equipos) {
      for (const cromo of equipo.cromos) {
        if (this.esBasico(cromo.tipoCromo.nombre)) continue;
        if (!grupos.has(cromo.tipoCromo.id)) grupos.set(cromo.tipoCromo.id, []);
        grupos.get(cromo.tipoCromo.id)!.push(cromo);
      }
    }

    const ordenados = data.tiposCromo
      .slice()
      .sort((a, b) => a.orden - b.orden);

    this.especiales = [];

    for (const tipo of ordenados) {
      const cromos = grupos.get(tipo.id);
      if (!cromos || cromos.length === 0) continue;
      this.especiales.push({
        tipo: tipo.nombre,
        cromos: cromos.sort((a, b) => this.compararNumero(a, b))
      });
    }

    for (const [id, cromos] of grupos) {
      if (ordenados.some(t => t.id === id)) continue;
      this.especiales.push({
        tipo: cromos[0].tipoCromo.nombre,
        cromos: cromos.sort((a, b) => this.compararNumero(a, b))
      });
    }
  }

  private cargarDetalle(id: number): void {
    this.cargando = true;
    this.albumService.getAlbumConCromos(id).subscribe({
      next: data => {
        this.album = data;
        this.construirEspeciales(data);
        this.cargando = false;
        this.cdr.detectChanges();

        if (this.miId) {
          this.cargarProgreso();
        }
      },
      error: () => {
        this.error = 'No se ha podido cargar el álbum.';
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  private cargarProgreso(): void {
    if (!this.miId || !this.album) return;

    this.usuarioAlbumService.getProgresoAlbum(this.miId, this.album.id).subscribe({
      next: data => {
        this.progreso = data;
        this.tenidosIds = new Set(data.cromosTenidos);
        this.cdr.detectChanges();
      },
      error: () => {
        this.progreso = null;
        this.cdr.detectChanges();
      }
    });
  }
}