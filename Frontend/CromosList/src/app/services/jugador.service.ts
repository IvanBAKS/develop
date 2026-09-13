import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Jugador {
  id: number;
  nombre: string;
  nombreCompleto: string;
  equipos: { id: number; nombre: string }[];
}

export interface JugadorPayload {
  nombre: string;
  nombreCompleto?: string;
  equipos: number[];
}

@Injectable({
  providedIn: 'root'
})
export class JugadorService {

  private http = inject(HttpClient);

  private apiUrl = '/api/Jugadores';

  getJugadores(): Observable<Jugador[]> {
    return this.http.get<Jugador[]>(this.apiUrl);
  }

  crearJugador(jugador: JugadorPayload): Observable<Jugador> {
    return this.http.post<Jugador>(this.apiUrl, jugador);
  }

  editarJugador(id: number, jugador: JugadorPayload): Observable<Jugador> {
    return this.http.put<Jugador>(`${this.apiUrl}/${id}`, jugador);
  }

  eliminarJugador(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
