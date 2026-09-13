import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatInputModule } from '@angular/material/input';
import { Observable, map, startWith } from 'rxjs';

@Component({
  selector: 'app-cromos',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatInputModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './cromos.html',
  styleUrl: './cromos.scss'
})
export class AnadirCromoComponent implements OnInit {

  cromoForm!: FormGroup;

  ediciones: any[] = [];
  albumes: any[] = [];
  equipos: any[] = [];
  jugadores: any[] = [];
  tiposCromo: any[] = [];

  filteredEquipos$!: Observable<any[]>;
  filteredJugadores$!: Observable<any[]>;
  filteredTiposCromo$!: Observable<any[]>;

  cargando = false;
  mensaje = '';
  error = '';

  private apiUrl = '/api';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.crearFormulario();
    this.cargarDatos();
  }

  private crearFormulario(): void {
    this.cromoForm = this.fb.group({
      numero: [
        '',
        Validators.required
      ],
      edicionId: [
        null,
        Validators.required
      ],
      albumId: [
        null,
        Validators.required
      ],
      equipo: [
        null,
        Validators.required
      ],
      jugador: [
        { value: null, disabled: true },
        Validators.required
      ],
      tipoCromo: [
        null,
        Validators.required
      ]
    });
  }

  private cargarDatos(): void {
    this.cargarEdiciones();
    this.cargarAlbumes();
    this.cargarEquipos();
    this.cargarTiposCromo();
  }

  private cargarEdiciones(): void {
    this.http
      .get<any[]>(`${this.apiUrl}/Ediciones`)
      .subscribe({
        next: data => {
          this.ediciones = data;
          this.cdr.detectChanges();
        },
        error: () => {
          this.error = 'No se han podido cargar las ediciones.';
        }
      });
  }

  private cargarAlbumes(): void {
    this.http
      .get<any[]>(`${this.apiUrl}/Albumes`)
      .subscribe({
        next: data => {
          this.albumes = data;
          this.cdr.detectChanges();
        },
        error: () => {
          this.error = 'No se han podido cargar los álbumes.';
        }
      });
  }

  private cargarEquipos(): void {
    this.http
      .get<any[]>(`${this.apiUrl}/Equipos`)
      .subscribe({
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
  }

  filtrarEquipos(value: any): any[] {
    const filtro = (typeof value === 'string' ? value : '').toLowerCase();
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

    this.jugadores = [];

    this.cromoForm.patchValue({ jugador: null });

    if (!equipoId) {
      this.cromoForm.get('jugador')?.disable();
      return;
    }

    this.http
      .get<any[]>(`${this.apiUrl}/Equipos/${equipoId}/Jugadores`)
      .subscribe({
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

  private cargarTiposCromo(): void {
    this.http
      .get<any[]>(`${this.apiUrl}/TiposCromo`)
      .subscribe({
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
    this.mensaje = '';
    this.error = '';

    const cromo = {
      numero: this.cromoForm.get('numero')?.value,
      edicionId: this.cromoForm.get('edicionId')?.value,
      albumId: this.cromoForm.get('albumId')?.value,
      equipoId: this.cromoForm.get('equipo')?.value?.id,
      jugadorId: this.cromoForm.get('jugador')?.value?.id,
      tipoCromoId: this.cromoForm.get('tipoCromo')?.value?.id
    };

    this.http
      .post(`${this.apiUrl}/Cromo`, cromo)
      .subscribe({
        next: () => {
          this.cargando = false;
          this.mensaje = 'Cromo añadido correctamente.';
          // Solo se limpia el jugador; numero, equipo, tipo de cromo,
          // álbum y edición se conservan para facilitar altas consecutivas.
          this.cromoForm.patchValue({ jugador: null });
          this.cromoForm.get('jugador')?.disable();
          this.cdr.detectChanges();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido añadir el cromo.';
        }
      });
  }

  cancelar(): void {
    this.cromoForm.reset();
    this.mensaje = '';
    this.error = '';
    this.cromoForm.get('jugador')?.disable();
    this.cdr.detectChanges();
  }
}
