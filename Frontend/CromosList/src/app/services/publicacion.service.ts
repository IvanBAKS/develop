import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Publicacion {
  id: number;
  temporada: string;
  nombre: string | null;
  fechaPublicacion: string;
}

@Injectable({
  providedIn: 'root'
})
export class PublicacionService {

  private http = inject(HttpClient);

  private apiUrl = '/api/Publicaciones';

  getPublicaciones(): Observable<Publicacion[]> {
    return this.http.get<Publicacion[]>(this.apiUrl);
  }
}