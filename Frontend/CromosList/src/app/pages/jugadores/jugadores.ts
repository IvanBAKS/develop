import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';
import { EquipoService, Equipo } from '../../services/equipo.service';
import { JugadorService, Jugador } from '../../services/jugador.service';

@Component({
  selector: 'app-jugadores',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './jugadores.html',
  styleUrl: './jugadores.scss'
})
export class JugadoresComponent implements OnInit {

  jugadorForm!: FormGroup;

  equipos: Equipo[] = [];
  jugadores: Jugador[] = [];

  editandoId: number | null = null;

  cargando = false;
  mensaje = '';
  error = '';

  constructor(
    private fb: FormBuilder,
    private equipoService: EquipoService,
    private jugadorService: JugadorService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.crearFormulario();
    this.cargarEquipos();
    this.cargarJugadores();
  }

  private crearFormulario(): void {
    this.jugadorForm = this.fb.group({
      nombre: [
        '',
        [
          Validators.required,
          Validators.maxLength(100)
        ]
      ],
      nombreCompleto: [
        '',
        [Validators.maxLength(150)]
      ],
      equipos: [
        [],
        [Validators.required]
      ]
    });
  }

  private cargarEquipos(): void {
    this.equipoService.getEquipos().subscribe({
      next: data => {
        this.equipos = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se han podido cargar los equipos.';
        this.cdr.detectChanges();
      }
    });
  }

  private cargarJugadores(): void {
    this.jugadorService.getJugadores().subscribe({
      next: data => {
        this.jugadores = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se han podido cargar los jugadores.';
        this.cdr.detectChanges();
      }
    });
  }

  get esModoEdicion(): boolean {
    return this.editandoId !== null;
  }

  editar(jugador: Jugador): void {
    this.editandoId = jugador.id;
    this.jugadorForm.patchValue({
      nombre: jugador.nombre,
      nombreCompleto: jugador.nombreCompleto,
      equipos: jugador.equipos.map(e => e.id)
    });
    this.mensaje = '';
    this.error = '';
  }

  guardar(): void {
    if (this.jugadorForm.invalid) {
      this.jugadorForm.markAllAsTouched();
      return;
    }

    this.cargando = true;
    this.mensaje = '';
    this.error = '';

    const payload = {
      nombre: this.jugadorForm.get('nombre')?.value,
      nombreCompleto: this.jugadorForm.get('nombreCompleto')?.value,
      equipos: this.jugadorForm.get('equipos')?.value
    };

    if (this.esModoEdicion) {
      this.jugadorService.editarJugador(this.editandoId!, payload).subscribe({
        next: (editado: Jugador) => {
          this.cargando = false;
          const idx = this.jugadores.findIndex(j => j.id === this.editandoId);
          if (idx !== -1) {
            this.jugadores[idx] = editado;
          }
          this.mensaje = 'Jugador actualizado correctamente.';
          this.cancelar();
          this.cdr.detectChanges();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido actualizar el jugador.';
          this.cdr.detectChanges();
        }
      });
    } else {
      this.jugadorService.crearJugador(payload).subscribe({
        next: (nuevo: Jugador) => {
          this.cargando = false;
          this.jugadores.push(nuevo);
          this.mensaje = 'Jugador añadido correctamente.';
          this.jugadorForm.reset();
          this.cdr.detectChanges();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido añadir el jugador.';
          this.cdr.detectChanges();
        }
      });
    }
  }

  eliminar(jugador: Jugador): void {
    if (!confirm(`¿Seguro que quieres eliminar el jugador "${jugador.nombre}"?`)) {
      return;
    }

    this.jugadorService.eliminarJugador(jugador.id).subscribe({
      next: () => {
        this.jugadores = this.jugadores.filter(j => j.id !== jugador.id);
        this.mensaje = 'Jugador eliminado correctamente.';
        this.error = '';
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se ha podido eliminar el jugador.';
        this.cdr.detectChanges();
      }
    });
  }

  cancelar(): void {
    this.editandoId = null;
    this.jugadorForm.reset({
      nombre: '',
      nombreCompleto: '',
      equipos: []
    });
    this.mensaje = '';
    this.error = '';
  }
}
