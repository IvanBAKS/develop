import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AlbumEquipo {
  id: number;
  nombre: string;
  orden: number;
}

export interface AlbumTipoCromo {
  id: number;
  nombre: string;
  orden: number;
}

export interface Album {
  id: number;
  nombre: string;
  temporada: string;
  editorial: { id: number; nombre: string };
  equipos: AlbumEquipo[];
  tiposCromo: AlbumTipoCromo[];
  contadorCromos: number;
}

export interface AlbumPayload {
  nombre: string;
  temporada: string;
  editorialId: number;
  equipos: Array<{ equipoId: number }>;
  tiposCromo: Array<{ tipoCromoId: number }>;
}

export interface AlbumCromo {
  id: number;
  numero: string;
  jugador: { id: number; nombre: string };
  equipo: { id: number; nombre: string };
  edicion: { id: number; nombre: string };
  tipoCromo: { id: number; nombre: string };
}

export interface AlbumEquipoConCromos {
  id: number;
  nombre: string;
  orden: number;
  cromos: AlbumCromo[];
}

export interface AlbumTipoCromoConOrden {
  id: number;
  nombre: string;
  orden: number;
}

export interface AlbumDetalle {
  id: number;
  nombre: string;
  temporada: string;
  equipos: AlbumEquipoConCromos[];
  tiposCromo: AlbumTipoCromoConOrden[];
}

@Injectable({
  providedIn: 'root'
})
export class AlbumService {

  private http = inject(HttpClient);

  private apiUrl = '/api/Albumes';

  getAlbumes(): Observable<Album[]> {
    return this.http.get<Album[]>(this.apiUrl);
  }

  crearAlbum(album: AlbumPayload): Observable<Album> {
    return this.http.post<Album>(this.apiUrl, album);
  }

  editarAlbum(id: number, album: AlbumPayload): Observable<Album> {
    return this.http.put<Album>(`${this.apiUrl}/${id}`, album);
  }

  eliminarAlbum(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getAlbumConCromos(id: number): Observable<AlbumDetalle> {
    return this.http.get<AlbumDetalle>(`${this.apiUrl}/${id}/Cromos`);
  }
}
