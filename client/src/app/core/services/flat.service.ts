import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { FlatDto, FlatDetailDto, CreateFlatRequest, JoinFlatRequest } from '../../models/flat.model';
import { PaginatedResult } from '../../models/paginated-result.model';

@Injectable({ providedIn: 'root' })
export class FlatService {
  constructor(private http: HttpClient) {}

  getMyFlats(): Observable<FlatDto[]> {
    return this.http.get<FlatDto[]>(`${environment.apiUrl}/flats/my`);
  }

  getAllFlats(page: number, pageSize: number): Observable<PaginatedResult<FlatDto>> {
    return this.http.get<PaginatedResult<FlatDto>>(
      `${environment.apiUrl}/flats?page=${page}&pageSize=${pageSize}`,
    );
  }

  createFlat(request: CreateFlatRequest): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/flats`, request);
  }

  getFlatById(id: string): Observable<FlatDetailDto> {
    return this.http.get<FlatDetailDto>(`${environment.apiUrl}/flats/${id}`);
  }

  getByAccessCode(code: string): Observable<FlatDto> {
    return this.http.get<FlatDto>(`${environment.apiUrl}/flats/by-access-code/${code}`);
  }

  joinFlat(request: JoinFlatRequest): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/flats/join`, request);
  }
}
