import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';
import { EquipoService, Equipo } from '../../services/equipo.service';

@Component({
  selector: 'app-equipos',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './equipos.html',
  styleUrl: './equipos.scss'
})
export class EquiposComponent implements OnInit {

  equipoForm!: FormGroup;

  equipos: Equipo[] = [];

  editandoId: number | null = null;

  cargando = false;
  mensaje = '';
  error = '';

  constructor(
    private fb: FormBuilder,
    private equipoService: EquipoService
  ) {}

  ngOnInit(): void {
    this.crearFormulario();
    this.cargarEquipos();
  }

  private crearFormulario(): void {
    this.equipoForm = this.fb.group({
      nombre: [
        '',
        [
          Validators.required,
          Validators.maxLength(100)
        ]
      ]
    });
  }

  private cargarEquipos(): void {
    this.equipoService.getEquipos().subscribe({
      next: data => {
        this.equipos = data;
      },
      error: () => {
        this.error = 'No se han podido cargar los equipos.';
      }
    });
  }

  get esModoEdicion(): boolean {
    return this.editandoId !== null;
  }

  editar(equipo: Equipo): void {
    this.editandoId = equipo.id;
    this.equipoForm.patchValue({ nombre: equipo.nombre });
    this.mensaje = '';
    this.error = '';
  }

  guardar(): void {
    if (this.equipoForm.invalid) {
      this.equipoForm.markAllAsTouched();
      return;
    }

    this.cargando = true;
    this.mensaje = '';
    this.error = '';

    const nombre = this.equipoForm.get('nombre')?.value;

    if (this.esModoEdicion) {
      this.equipoService.editarEquipo(this.editandoId!, { nombre }).subscribe({
        next: (editado: Equipo) => {
          this.cargando = false;
          const idx = this.equipos.findIndex(e => e.id === this.editandoId);
          if (idx !== -1) {
            this.equipos[idx] = editado;
          }
          this.mensaje = 'Equipo actualizado correctamente.';
          this.cancelar();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido actualizar el equipo.';
        }
      });
    } else {
      this.equipoService.crearEquipo({ nombre }).subscribe({
        next: (nuevo: Equipo) => {
          this.cargando = false;
          this.equipos.push(nuevo);
          this.mensaje = 'Equipo añadido correctamente.';
          this.equipoForm.reset();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido añadir el equipo.';
        }
      });
    }
  }

  cancelar(): void {
    this.editandoId = null;
    this.equipoForm.reset();
    this.mensaje = '';
    this.error = '';
  }
}
