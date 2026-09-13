import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Editorial {
  id: number;
  nombre: string;
}

@Injectable({
  providedIn: 'root'
})
export class EditorialService {

  private http = inject(HttpClient);

  private apiUrl = '/api/Editoriales';

  getEditoriales(): Observable<Editorial[]> {
    return this.http.get<Editorial[]>(this.apiUrl);
  }

  crearEditorial(editorial: { nombre: string }): Observable<Editorial> {
    return this.http.post<Editorial>(this.apiUrl, editorial);
  }

  editarEditorial(id: number, editorial: { nombre: string }): Observable<Editorial> {
    return this.http.put<Editorial>(`${this.apiUrl}/${id}`, editorial);
  }

  eliminarEditorial(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
