import { Component, Inject, ChangeDetectorRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Observable, map, startWith } from 'rxjs';
import { EquipoService, Equipo } from '../../services/equipo.service';
import { EdicionService, Edicion } from '../../services/edicion.service';
import { TipoCromoService, TipoCromo } from '../../services/tipo-cromo.service';
import { AlbumService, Album, AlbumCromo } from '../../services/album.service';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';

export interface EditarCromoData {
  cromo: AlbumCromo;
  albumId: number;
}

@Component({
  selector: 'app-cromo-editar',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatAutocompleteModule,
    MatInputModule,
    MatProgressSpinnerModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './cromo-editar.html',
  styleUrl: './cromo-editar.scss'
})
export class CromoEditarComponent implements OnInit {

  cromoForm!: FormGroup;

  equipos: Equipo[] = [];
  ediciones: Edicion[] = [];
  tiposCromo: TipoCromo[] = [];
  albumes: Album[] = [];
  jugadores: any[] = [];

  filteredEquipos$!: Observable<any[]>;
  filteredJugadores$!: Observable<any[]>;
  filteredTiposCromo$!: Observable<any[]>;

  cargando = false;
  error = '';

  private apiUrl = '/api';

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EditarCromoData,
    private dialogRef: MatDialogRef<CromoEditarComponent>,
    private fb: FormBuilder,
    private http: HttpClient,
    private equipoService: EquipoService,
    private edicionService: EdicionService,
    private tipoCromoService: TipoCromoService,
    private albumService: AlbumService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.crearFormulario();
    this.cargarDatosMaestros();
  }

  private crearFormulario(): void {
    const cromo = this.data.cromo;

    this.cromoForm = this.fb.group({
      numero: [cromo.numero, Validators.required],
      equipo: [{ ...cromo.equipo }, Validators.required],
      jugador: [{ value: { ...cromo.jugador }, disabled: false }, Validators.required],
      edicionId: [cromo.edicion.id, Validators.required],
      tipoCromo: [{ ...cromo.tipoCromo }, Validators.required],
      albumId: [this.data.albumId, Validators.required]
    });
  }

  private cargarDatosMaestros(): void {
    this.equipoService.getEquipos().subscribe({
      next: data => {
        this.equipos = data;
        this.filteredEquipos$ = this.cromoForm.get('equipo')!.valueChanges.pipe(
          startWith(''),
          map(value => this.filtrarEquipos(value))
        );
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se han podido cargar los equipos.';
      }
    });

    this.edicionService.getEdiciones().subscribe({
      next: data => {
        this.ediciones = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se han podido cargar las ediciones.';
      }
    });

    this.tipoCromoService.getTiposCromo().subscribe({
      next: data => {
        this.tiposCromo = data;
        this.filteredTiposCromo$ = this.cromoForm.get('tipoCromo')!.valueChanges.pipe(
          startWith(''),
          map(value => this.filtrarTiposCromo(value))
        );
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se han podido cargar los tipos de cromo.';
      }
    });

    this.albumService.getAlbumes().subscribe({
      next: data => {
        this.albumes = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se han podido cargar los álbumes.';
      }
    });

    this.cargarJugadoresDelEquipo(this.data.cromo.equipo.id);
  }

  filtrarEquipos(value: any): any[] {
    const filtro = (typeof value === 'string' ? value : value?.nombre ?? '').toLowerCase();
    return this.equipos.filter(e => e.nombre.toLowerCase().includes(filtro));
  }

  onEquipoInput(value: string): void {
    this.cromoForm.get('equipo')?.setValue(value);
  }

  onEquipoSelected(equipo: any): void {
    this.cromoForm.get('equipo')?.setValue(equipo);
    this.onEquipoChange();
  }

  displayEquipo(equipo: any): string {
    return equipo?.nombre ?? '';
  }

  onEquipoChange(): void {
    const equipo = this.cromoForm.get('equipo')?.value;
    const equipoId = equipo?.id;
    if (!equipoId) return;
    this.cargarJugadoresDelEquipo(equipoId);
  }

  private cargarJugadoresDelEquipo(equipoId: number): void {
    this.jugadores = [];
    this.cromoForm.get('jugador')?.setValue(null);
    this.cromoForm.get('jugador')?.disable();

    this.http.get<any[]>(`${this.apiUrl}/Equipos/${equipoId}/Jugadores`).subscribe({
      next: data => {
        this.jugadores = data;
        this.filteredJugadores$ = this.cromoForm.get('jugador')!.valueChanges.pipe(
          startWith(''),
          map(value => this.filtrarJugadores(value))
        );
        this.cromoForm.get('jugador')?.enable();
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se han podido cargar los jugadores.';
      }
    });
  }

  filtrarJugadores(value: any): any[] {
    const filtro = (typeof value === 'string' ? value : '').toLowerCase();
    return this.jugadores.filter(j =>
      ((j.nombreCompleto || j.nombre) as string).toLowerCase().includes(filtro)
    );
  }

  onJugadorInput(value: string): void {
    this.cromoForm.get('jugador')?.setValue(value);
  }

  onJugadorSelected(jugador: any): void {
    this.cromoForm.get('jugador')?.setValue(jugador);
  }

  displayJugador(jugador: any): string {
    if (!jugador) return '';
    return typeof jugador === 'string' ? jugador : (jugador.nombreCompleto || jugador.nombre);
  }

  filtrarTiposCromo(value: any): any[] {
    const filtro = (typeof value === 'string' ? value : value?.nombre ?? '').toLowerCase();
    return this.tiposCromo.filter(t => t.nombre.toLowerCase().includes(filtro));
  }

  onTipoCromoInput(value: string): void {
    this.cromoForm.get('tipoCromo')?.setValue(value);
  }

  onTipoCromoSelected(tipo: any): void {
    this.cromoForm.get('tipoCromo')?.setValue(tipo);
  }

  displayTipoCromo(tipo: any): string {
    return tipo?.nombre ?? '';
  }

  guardar(): void {
    if (this.cromoForm.invalid) {
      this.cromoForm.markAllAsTouched();
      return;
    }

    this.cargando = true;
    this.error = '';

    const payload = {
      numero: this.cromoForm.get('numero')?.value,
      edicionId: this.cromoForm.get('edicionId')?.value,
      albumId: this.cromoForm.get('albumId')?.value,
      equipoId: this.cromoForm.get('equipo')?.value?.id,
      jugadorId: this.cromoForm.get('jugador')?.value?.id,
      tipoCromoId: this.cromoForm.get('tipoCromo')?.value?.id,
      publicacionId: null,
      coleccionId: null
    };

    this.http.put(`${this.apiUrl}/Cromo/${this.data.cromo.id}`, payload).subscribe({
      next: () => {
        this.cargando = false;
        this.dialogRef.close(true);
      },
      error: () => {
        this.cargando = false;
        this.error = 'No se ha podido actualizar el cromo.';
        this.cdr.detectChanges();
      }
    });
  }

  cancelar(): void {
    this.dialogRef.close(false);
  }
}
