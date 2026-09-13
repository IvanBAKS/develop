import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsuarioCromoService {

  private http = inject(HttpClient);

  private apiUrl = '/api/UsuariosCromos';

  marcarCromo(usuarioId: number, cromoId: number): Observable<unknown> {
    return this.http.post(this.apiUrl, { usuarioId, cromoId });
  }

  desmarcarCromo(usuarioId: number, cromoId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${usuarioId}/${cromoId}`);
  }
}