import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ColeccionSuscripcion {
  albumId: number;
  nombre: string;
  temporada: string;
  editorial: { id: number; nombre: string };
  totalCromos: number;
  tenidos: number;
  faltan: number;
  completado: boolean;
}

export interface ColeccionProgreso {
  id: number;
  nombre: string;
  temporada: string;
  suscrito: boolean;
  totalCromos: number;
  tenidos: number;
  faltan: number;
  cromosTenidos: number[];
}

export interface SuscribirPayload {
  usuarioId: number;
  albumId: number;
}

@Injectable({
  providedIn: 'root'
})
export class UsuarioAlbumService {

  private http = inject(HttpClient);

  private apiUrl = '/api/UsuariosAlbumes';

  listarSuscripciones(usuarioId: number): Observable<ColeccionSuscripcion[]> {
    return this.http.get<ColeccionSuscripcion[]>(`${this.apiUrl}/${usuarioId}`);
  }

  getProgresoAlbum(usuarioId: number, albumId: number): Observable<ColeccionProgreso> {
    return this.http.get<ColeccionProgreso>(`${this.apiUrl}/${usuarioId}/albumes/${albumId}`);
  }

  suscribir(payload: SuscribirPayload): Observable<unknown> {
    return this.http.post(this.apiUrl, payload);
  }

  desuscribir(usuarioId: number, albumId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${usuarioId}/${albumId}`);
  }
}