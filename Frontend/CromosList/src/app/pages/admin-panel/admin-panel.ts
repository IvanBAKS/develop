import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MATERIAL_IMPORTS } from '../../shared/material.imports';
import { AdminService, ResumenPanel, CromoBusqueda, UsuarioPanel } from '../../services/admin.service';
import { AuthService } from '../../services/auth.service';
import { of, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-admin-panel',
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatProgressSpinnerModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.scss'
})
export class AdminPanelComponent implements OnInit, OnDestroy {

  cargando = true;
  error = '';
  resumen: ResumenPanel | null = null;

  usuarios: UsuarioPanel[] = [];
  cargandoUsuarios = false;

  busqueda = new FormControl('');
  buscando = false;
  resultados: CromoBusqueda[] = [];
  buscado = '';
  sinResultados = false;

  private subBusqueda?: Subscription;

  tarjetas: { clave: string; titulo: string; descripcion: string; icono: string; ruta?: string }[] = [
    { clave: 'cromos', titulo: 'Cromos', descripcion: 'Personajes y tarjetas', icono: 'style', ruta: '/cromos' },
    { clave: 'jugadores', titulo: 'Jugadores', descripcion: 'Deportistas registrados', icono: 'person', ruta: '/jugadores' },
    { clave: 'equipos', titulo: 'Equipos', descripcion: 'Clubes y selecciones', icono: 'groups', ruta: '/equipos' },
    { clave: 'ediciones', titulo: 'Ediciones', descripcion: 'Lanzamientos y series', icono: 'auto_awesome', ruta: '/ediciones' },
    { clave: 'editoriales', titulo: 'Editoriales', descripcion: 'Empresas editoras', icono: 'storefront', ruta: '/editoriales' },
    { clave: 'publicaciones', titulo: 'Publicaciones', descripcion: 'Entregas publicadas', icono: 'newspaper' },
    { clave: 'tiposCromo', titulo: 'Tipos de cromo', descripcion: 'Categorías especiales', icono: 'category', ruta: '/tipos-cromo' },
    { clave: 'albumes', titulo: 'Álbumes', descripcion: 'Colecciones armadas', icono: 'album', ruta: '/albumes' },
    { clave: 'colecciones', titulo: 'Colecciones', descripcion: 'Conjuntos definidos', icono: 'collections_bookmark', ruta: '/albumes' },
    { clave: 'usuarios', titulo: 'Usuarios', descripcion: 'Cuentas activas', icono: 'group' }
  ];

  constructor(
    private adminService: AdminService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.cargarResumen();
    this.cargarUsuarios();

    this.subBusqueda = this.busqueda.valueChanges.pipe(
      debounceTime(350),
      distinctUntilChanged(),
      switchMap(q => {
        const termino = (q ?? '').trim();
        if (!termino) {
          this.resultados = [];
          this.buscado = '';
          this.sinResultados = false;
          this.buscando = false;
          this.cdr.detectChanges();
          return of([]);
        }
        this.buscando = true;
        this.sinResultados = false;
        return this.adminService.buscarCromos(termino);
      })
    ).subscribe({
      next: cromos => {
        this.buscando = false;
        this.resultados = cromos;
        this.buscado = (this.busqueda.value ?? '').trim();
        this.sinResultados = this.buscado !== '' && cromos.length === 0;
        this.cdr.detectChanges();
      },
      error: () => {
        this.buscando = false;
        this.error = 'No se ha podido realizar la búsqueda.';
        this.cdr.detectChanges();
      }
    });
  }

  ngOnDestroy(): void {
    this.subBusqueda?.unsubscribe();
  }

  limpiar(): void {
    this.busqueda.setValue('');
    this.resultados = [];
    this.buscado = '';
    this.sinResultados = false;
  }

  cargarResumen(): void {
    this.cargando = true;
    this.error = '';

    this.adminService.cargarResumen().subscribe({
      next: resumen => {
        this.resumen = resumen;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.cargando = false;
        this.error = 'No se ha podido cargar el panel de administración.';
        this.cdr.detectChanges();
      }
    });
  }

  valor(clave: string): number {
    if (!this.resumen) {
      return 0;
    }
    return (this.resumen as unknown as Record<string, number>)[clave] ?? 0;
  }

  cargarUsuarios(): void {
    this.cargandoUsuarios = true;
    this.adminService.listarUsuarios().subscribe({
      next: usuarios => {
        this.usuarios = usuarios;
        this.cargandoUsuarios = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.cargandoUsuarios = false;
        this.error = 'No se ha podido cargar la lista de usuarios.';
        this.cdr.detectChanges();
      }
    });
  }

  esYo(id: number): boolean {
    return this.authService.getMiId() === id;
  }

  promover(id: number): void {
    this.adminService.promoverAdmin(id).subscribe({
      next: () => this.cargarUsuarios(),
      error: () => {
        this.error = 'No se ha podido promover el usuario.';
        this.cdr.detectChanges();
      }
    });
  }

  revocar(id: number): void {
    this.adminService.revocarAdmin(id).subscribe({
      next: () => this.cargarUsuarios(),
      error: () => {
        this.error = 'No se ha podido revocar el rol de administrador.';
        this.cdr.detectChanges();
      }
    });
  }
}
