import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TipoCromo {
  id: number;
  nombre: string;
}

@Injectable({
  providedIn: 'root'
})
export class TipoCromoService {

  private http = inject(HttpClient);

  private apiUrl = '/api/TiposCromo';

  getTiposCromo(): Observable<TipoCromo[]> {
    return this.http.get<TipoCromo[]>(this.apiUrl);
  }

  crearTipoCromo(tipoCromo: { nombre: string }): Observable<TipoCromo> {
    return this.http.post<TipoCromo>(this.apiUrl, tipoCromo);
  }

  editarTipoCromo(id: number, tipoCromo: { nombre: string }): Observable<TipoCromo> {
    return this.http.put<TipoCromo>(`${this.apiUrl}/${id}`, tipoCromo);
  }

  eliminarTipoCromo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}