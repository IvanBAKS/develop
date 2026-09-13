import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';
import { EditorialService, Editorial } from '../../services/editorial.service';

@Component({
  selector: 'app-editoriales',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './editoriales.html',
  styleUrl: './editoriales.scss'
})
export class EditorialesComponent implements OnInit {

  editorialForm!: FormGroup;

  editoriales: Editorial[] = [];

  editandoId: number | null = null;

  cargando = false;
  mensaje = '';
  error = '';

  constructor(
    private fb: FormBuilder,
    private editorialService: EditorialService
  ) {}

  ngOnInit(): void {
    this.crearFormulario();
    this.cargarEditoriales();
  }

  private crearFormulario(): void {
    this.editorialForm = this.fb.group({
      nombre: [
        '',
        [
          Validators.required,
          Validators.maxLength(100)
        ]
      ]
    });
  }

  private cargarEditoriales(): void {
    this.editorialService.getEditoriales().subscribe({
      next: data => {
        this.editoriales = data;
      },
      error: () => {
        this.error = 'No se han podido cargar las editoriales.';
      }
    });
  }

  get esModoEdicion(): boolean {
    return this.editandoId !== null;
  }

  editar(editorial: Editorial): void {
    this.editandoId = editorial.id;
    this.editorialForm.patchValue({ nombre: editorial.nombre });
    this.mensaje = '';
    this.error = '';
  }

  guardar(): void {
    if (this.editorialForm.invalid) {
      this.editorialForm.markAllAsTouched();
      return;
    }

    this.cargando = true;
    this.mensaje = '';
    this.error = '';

    const nombre = this.editorialForm.get('nombre')?.value;

    if (this.esModoEdicion) {
      this.editorialService.editarEditorial(this.editandoId!, { nombre }).subscribe({
        next: (editado: Editorial) => {
          this.cargando = false;
          const idx = this.editoriales.findIndex(e => e.id === this.editandoId);
          if (idx !== -1) {
            this.editoriales[idx] = editado;
          }
          this.mensaje = 'Editorial actualizada correctamente.';
          this.cancelar();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido actualizar la editorial.';
        }
      });
    } else {
      this.editorialService.crearEditorial({ nombre }).subscribe({
        next: (nueva: Editorial) => {
          this.cargando = false;
          this.editoriales.push(nueva);
          this.mensaje = 'Editorial añadida correctamente.';
          this.editorialForm.reset();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido añadir la editorial.';
        }
      });
    }
  }

  eliminar(editorial: Editorial): void {
    this.editorialService.eliminarEditorial(editorial.id).subscribe({
      next: () => {
        this.editoriales = this.editoriales.filter(e => e.id !== editorial.id);
        this.mensaje = 'Editorial eliminada correctamente.';
        this.error = '';
      },
      error: () => {
        this.mensaje = '';
        this.error = 'No se ha podido eliminar la editorial.';
      }
    });
  }

  cancelar(): void {
    this.editandoId = null;
    this.editorialForm.reset();
    this.mensaje = '';
    this.error = '';
  }
}
