import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormBuilder, FormGroup, FormArray, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';
import { AlbumService, Album } from '../../services/album.service';
import { EditorialService, Editorial } from '../../services/editorial.service';
import { EquipoService, Equipo } from '../../services/equipo.service';
import { TipoCromoService, TipoCromo } from '../../services/tipo-cromo.service';

@Component({
  selector: 'app-albumes',
  imports: [
    CommonModule,
    RouterLink,
    MatTooltipModule,
    ReactiveFormsModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './albumes.html',
  styleUrl: './albumes.scss'
})
export class AlbumesComponent implements OnInit {

  albumForm!: FormGroup;

  albumes: Album[] = [];
  editoriales: Editorial[] = [];
  equipos: Equipo[] = [];
  tiposCromo: TipoCromo[] = [];

  editandoId: number | null = null;

  cargando = false;
  mensaje = '';
  error = '';

  constructor(
    private fb: FormBuilder,
    private albumService: AlbumService,
    private editorialService: EditorialService,
    private equipoService: EquipoService,
    private tipoCromoService: TipoCromoService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.crearFormulario();
    this.cargarAlbumes();
    this.cargarEditoriales();
    this.cargarEquipos();
    this.cargarTiposCromo();
  }

  get equiposForm(): FormArray {
    return this.albumForm.get('equipos') as FormArray;
  }

  get tiposCromoForm(): FormArray {
    return this.albumForm.get('tiposCromo') as FormArray;
  }

  equipoControl(i: number): FormControl {
    return this.equiposForm.at(i) as FormControl;
  }

  tipoCromoControl(i: number): FormControl {
    return this.tiposCromoForm.at(i) as FormControl;
  }

  private crearFormulario(): void {
    this.albumForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      temporada: ['', [Validators.required, Validators.maxLength(50)]],
      editorialId: [null, [Validators.required]],
      equipos: this.fb.array([], [Validators.required]),
      tiposCromo: this.fb.array([], [Validators.required])
    });
  }

  private cargarAlbumes(): void {
    this.albumService.getAlbumes().subscribe({
      next: data => { this.albumes = data; this.cdr.detectChanges(); },
      error: () => { this.error = 'No se han podido cargar los álbumes.'; this.cdr.detectChanges(); }
    });
  }

  private cargarEditoriales(): void {
    this.editorialService.getEditoriales().subscribe({
      next: data => { this.editoriales = data; this.cdr.detectChanges(); },
      error: () => { this.error = 'No se han podido cargar las editoriales.'; this.cdr.detectChanges(); }
    });
  }

  private cargarEquipos(): void {
    this.equipoService.getEquipos().subscribe({
      next: data => { this.equipos = data; this.cdr.detectChanges(); },
      error: () => { this.error = 'No se han podido cargar los equipos.'; this.cdr.detectChanges(); }
    });
  }

  private cargarTiposCromo(): void {
    this.tipoCromoService.getTiposCromo().subscribe({
      next: data => { this.tiposCromo = data; this.cdr.detectChanges(); },
      error: () => { this.error = 'No se han podido cargar los tipos de cromo.'; this.cdr.detectChanges(); }
    });
  }

  agregarEquipo(): void {
    this.equiposForm.push(this.fb.control(null, Validators.required));
  }

  agregarTipoCromo(): void {
    this.tiposCromoForm.push(this.fb.control(null, Validators.required));
  }

  quitarEquipo(index: number): void {
    this.equiposForm.removeAt(index);
  }

  quitarTipoCromo(index: number): void {
    this.tiposCromoForm.removeAt(index);
  }

  moverEquipo(index: number, direccion: number): void {
    const nuevoIndex = index + direccion;
    if (nuevoIndex < 0 || nuevoIndex >= this.equiposForm.length) return;
    this._mover(this.equiposForm, index, nuevoIndex);
  }

  moverTipoCromo(index: number, direccion: number): void {
    const nuevoIndex = index + direccion;
    if (nuevoIndex < 0 || nuevoIndex >= this.tiposCromoForm.length) return;
    this._mover(this.tiposCromoForm, index, nuevoIndex);
  }

  private _mover(array: FormArray, de: number, a: number): void {
    const valor = array.at(de).value;
    array.removeAt(de);
    array.insert(a, this.fb.control(valor, Validators.required));
  }

  get esModoEdicion(): boolean {
    return this.editandoId !== null;
  }

  editar(album: Album): void {
    this.editandoId = album.id;
    this.albumForm.patchValue({
      nombre: album.nombre,
      temporada: album.temporada,
      editorialId: album.editorial.id
    });

    this.equiposForm.clear();
    album.equipos
      .slice()
      .sort((a, b) => a.orden - b.orden)
      .forEach(e => this.equiposForm.push(this.fb.control(e.id, Validators.required)));

    this.tiposCromoForm.clear();
    album.tiposCromo
      .slice()
      .sort((a, b) => a.orden - b.orden)
      .forEach(t => this.tiposCromoForm.push(this.fb.control(t.id, Validators.required)));

    this.mensaje = '';
    this.error = '';
    this.cdr.detectChanges();
  }

  guardar(): void {
    if (this.albumForm.invalid) {
      this.albumForm.markAllAsTouched();
      return;
    }

    this.cargando = true;
    this.mensaje = '';
    this.error = '';

    const formValue = {
      nombre: this.albumForm.get('nombre')?.value,
      temporada: this.albumForm.get('temporada')?.value,
      editorialId: this.albumForm.get('editorialId')?.value,
      equipos: this.equiposForm.value.map((id: number) => ({ equipoId: id })),
      tiposCromo: this.tiposCromoForm.value.map((id: number) => ({ tipoCromoId: id }))
    };

    if (this.esModoEdicion) {
      this.albumService.editarAlbum(this.editandoId!, formValue).subscribe({
        next: () => {
          this.cargando = false;
          this.mensaje = 'Álbum actualizado correctamente.';
          this.cargarAlbumes();
          this.cancelar();
          this.cdr.detectChanges();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido actualizar el álbum.';
          this.cdr.detectChanges();
        }
      });
    } else {
      this.albumService.crearAlbum(formValue).subscribe({
        next: () => {
          this.cargando = false;
          this.mensaje = 'Álbum añadido correctamente.';
          this.cargarAlbumes();
          this.cancelar();
          this.cdr.detectChanges();
        },
        error: () => {
          this.cargando = false;
          this.error = 'No se ha podido añadir el álbum.';
          this.cdr.detectChanges();
        }
      });
    }
  }

  eliminar(album: Album): void {
    this.albumService.eliminarAlbum(album.id).subscribe({
      next: () => {
        this.albumes = this.albumes.filter(a => a.id !== album.id);
        this.mensaje = 'Álbum eliminado correctamente.';
        this.error = '';
        this.cdr.detectChanges();
      },
      error: () => {
        this.mensaje = '';
        this.error = 'No se ha podido eliminar el álbum.';
        this.cdr.detectChanges();
      }
    });
  }

  cancelar(): void {
    this.editandoId = null;
    this.albumForm.reset();
    this.equiposForm.clear();
    this.tiposCromoForm.clear();
    this.mensaje = '';
    this.error = '';
    this.cdr.detectChanges();
  }
}
