import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Edicion {
  id: number;
  nombre: string;
  temporada: string;
  fechaPublicacion: string;
}

@Injectable({
  providedIn: 'root'
})
export class EdicionService {

  private http = inject(HttpClient);

  private apiUrl = '/api/Ediciones';

  getEdiciones(): Observable<Edicion[]> {
    return this.http.get<Edicion[]>(this.apiUrl);
  }
}