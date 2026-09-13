import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export interface ResumenPanel {
  cromos: number;
  jugadores: number;
  equipos: number;
  ediciones: number;
  editoriales: number;
  publicaciones: number;
  tiposCromo: number;
  albumes: number;
  colecciones: number;
  usuarios: number;
  cromosPorUsuario: number;
}

export interface TarjetaPanel {
  clave: keyof ResumenPanel;
  titulo: string;
  descripcion: string;
  icono: string;
  ruta?: string;
}

export interface CromoBusqueda {
  id: number;
  numero: string;
  jugador: { id: number; nombre: string; nombreCompleto: string } | null;
  equipo: { id: number; nombre: string } | null;
  album?: { id: number; nombre: string; temporada: string } | null;
  tipoCromo?: { id: number; nombre: string } | null;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  private http = inject(HttpClient);

  private apiUrl = '/api';

  cargarResumen(): Observable<ResumenPanel> {
    return forkJoin({
      cromos: this.http.get<any[]>(`${this.apiUrl}/Cromo`),
      jugadores: this.http.get<any[]>(`${this.apiUrl}/Jugadores`),
      equipos: this.http.get<any[]>(`${this.apiUrl}/Equipos`),
      ediciones: this.http.get<any[]>(`${this.apiUrl}/Ediciones`),
      editoriales: this.http.get<any[]>(`${this.apiUrl}/Editoriales`),
      publicaciones: this.http.get<any[]>(`${this.apiUrl}/Publicaciones`),
      tiposCromo: this.http.get<any[]>(`${this.apiUrl}/TiposCromo`),
      albumes: this.http.get<any[]>(`${this.apiUrl}/Albumes`),
      colecciones: this.http.get<any[]>(`${this.apiUrl}/Colecciones`),
      usuarios: this.http.get<any[]>(`${this.apiUrl}/Usuarios`),
      usuariosCromo: this.http.get<any[]>(`${this.apiUrl}/UsuariosCromos`)
    }).pipe(
      map(({ cromos, jugadores, equipos, ediciones, editoriales, publicaciones, tiposCromo, albumes, colecciones, usuarios, usuariosCromo }) => ({
        cromos: cromos.length,
        jugadores: jugadores.length,
        equipos: equipos.length,
        ediciones: ediciones.length,
        editoriales: editoriales.length,
        publicaciones: publicaciones.length,
        tiposCromo: tiposCromo.length,
        albumes: albumes.length,
        colecciones: colecciones.length,
        usuarios: usuarios.length,
        cromosPorUsuario: usuarios.length > 0
          ? Math.round(usuariosCromo.length / usuarios.length)
          : 0
      }))
    );
  }

  buscarCromos(q: string): Observable<CromoBusqueda[]> {
    return this.http.get<CromoBusqueda[]>(`${this.apiUrl}/Cromo`, {
      params: q ? { q } : {}
    });
  }
}
