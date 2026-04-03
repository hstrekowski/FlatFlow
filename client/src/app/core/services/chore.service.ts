import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ChoreDto,
  ChoreDetailDto,
  AddChoreRequest,
  UpdateChoreRequest,
  AddChoreAssignmentRequest,
} from '../../models/chore.model';

@Injectable({ providedIn: 'root' })
export class ChoreService {
  constructor(private http: HttpClient) {}

  getChoresByFlatId(flatId: string): Observable<ChoreDto[]> {
    return this.http.get<ChoreDto[]>(`${environment.apiUrl}/flats/${flatId}/chores`);
  }

  getChoreById(flatId: string, choreId: string): Observable<ChoreDetailDto> {
    return this.http.get<ChoreDetailDto>(`${environment.apiUrl}/flats/${flatId}/chores/${choreId}`);
  }

  addChore(flatId: string, request: AddChoreRequest): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/flats/${flatId}/chores`, request);
  }

  updateChore(flatId: string, choreId: string, request: UpdateChoreRequest): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/flats/${flatId}/chores/${choreId}`, request);
  }

  removeChore(flatId: string, choreId: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/flats/${flatId}/chores/${choreId}`);
  }

  addAssignment(flatId: string, choreId: string, request: AddChoreAssignmentRequest): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/flats/${flatId}/chores/${choreId}/assignments`, request);
  }

  removeAssignment(flatId: string, choreId: string, assignmentId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.apiUrl}/flats/${flatId}/chores/${choreId}/assignments/${assignmentId}`,
    );
  }

  completeAssignment(flatId: string, choreId: string, assignmentId: string): Observable<void> {
    return this.http.post<void>(
      `${environment.apiUrl}/flats/${flatId}/chores/${choreId}/assignments/${assignmentId}/complete`,
      {},
    );
  }

  reopenAssignment(flatId: string, choreId: string, assignmentId: string): Observable<void> {
    return this.http.post<void>(
      `${environment.apiUrl}/flats/${flatId}/chores/${choreId}/assignments/${assignmentId}/reopen`,
      {},
    );
  }
}
