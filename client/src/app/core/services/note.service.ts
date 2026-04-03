import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { NoteDto, AddNoteRequest, UpdateNoteRequest } from '../../models/note.model';
import { PaginatedResult } from '../../models/paginated-result.model';

@Injectable({ providedIn: 'root' })
export class NoteService {
  constructor(private http: HttpClient) {}

  getNotesByFlatId(flatId: string, page: number, pageSize: number): Observable<PaginatedResult<NoteDto>> {
    return this.http.get<PaginatedResult<NoteDto>>(
      `${environment.apiUrl}/flats/${flatId}/notes?page=${page}&pageSize=${pageSize}`,
    );
  }

  addNote(flatId: string, request: AddNoteRequest): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/flats/${flatId}/notes`, request);
  }

  updateNote(flatId: string, noteId: string, request: UpdateNoteRequest): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/flats/${flatId}/notes/${noteId}`, request);
  }

  removeNote(flatId: string, noteId: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/flats/${flatId}/notes/${noteId}`);
  }
}
