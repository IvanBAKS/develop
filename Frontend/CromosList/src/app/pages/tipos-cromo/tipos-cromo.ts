import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';
import { TipoCromoService, TipoCromo } from '../../services/tipo-cromo.service';

@Component({
  selector: 'app-tipos-cromo',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './tipos-cromo.html',
  styleUrl: './tipos-cromo.scss'
})
export class TiposCromoComponent implements OnInit {

  tipoCromoForm!: FormGroup;

  tiposCromo: TipoCromo[] = [];

  editandoId: number | null = null;

  cargando = false;
  mensaje = '';
  error = '';

  constructor(
    private fb: FormBuilder,
    private tipoCromoService: TipoCromoService
  ) {}

  ngOnInit(): void {
    this.crearFormulario();
    this.cargarTiposCromo();
  }

  private crearFormulario(): void {
    this.tipoCromoForm = this.fb.group({
      nombre: [
        '',
        [
          Validators.required,
          Validators.maxLength(100)
        ]
      ]
    });
  }

  private cargarTiposCromo(): void {
    this.tipoCromoService.getTiposCromo().subscribe({
      next: data => {
        this.tiposCromo = data;
      },
      error: () => {
        this.error = 'No se han podido cargar los tipos de cromo.';
      }
    });
  }

  get esModoEdicion(): boolean {
    return this.editandoId !== null;
  }

  editar(tipoCromo: TipoCromo): void {
    this.editandoId = tipoCromo.id;
    this.tipoCromoForm.patchValue({ nombre: tipoCromo.nombre });
    this.mensaje = '';
    this.error = '';
  }

  guardar(): void {
    if (this.tipoCromoForm.invalid) {
      this.tipoCromoForm.markAllAsTouched();
      return;
    }

    this.cargando = true;
    this.mensaje = '';
    this.error = '';

    const nombre = this.tipoCromoForm.get('nombre')?.value;

    if (this.esModoEdicion) {
      this.tipoCromoService.editarTipoCromo(this.editandoId!, { nombre }).subscribe({
        next: (editado: TipoCromo) => {
          this.cargando = false;
          const idx = this.tiposCromo.findIndex(t => t.id === this.editandoId);
          if (idx !== -1) {
            this.tiposCromo[idx] = editado;
          }
          this.mensaje = 'Tipo de cromo actualizado correctamente.';
          this.cancelar();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido actualizar el tipo de cromo.';
        }
      });
    } else {
      this.tipoCromoService.crearTipoCromo({ nombre }).subscribe({
        next: (nuevo: TipoCromo) => {
          this.cargando = false;
          this.tiposCromo.push(nuevo);
          this.mensaje = 'Tipo de cromo añadido correctamente.';
          this.tipoCromoForm.reset();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido añadir el tipo de cromo.';
        }
      });
    }
  }

  eliminar(tipoCromo: TipoCromo): void {
    this.tipoCromoService.eliminarTipoCromo(tipoCromo.id).subscribe({
      next: () => {
        this.tiposCromo = this.tiposCromo.filter(t => t.id !== tipoCromo.id);
        this.mensaje = 'Tipo de cromo eliminado correctamente.';
        this.error = '';
      },
      error: () => {
        this.mensaje = '';
        this.error = 'No se ha podido eliminar el tipo de cromo.';
      }
    });
  }

  cancelar(): void {
    this.editandoId = null;
    this.tipoCromoForm.reset();
    this.mensaje = '';
    this.error = '';
  }
}
