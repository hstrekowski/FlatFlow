import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TenantDto } from '../../models/tenant.model';

@Injectable({ providedIn: 'root' })
export class TenantService {
  constructor(private http: HttpClient) {}

  getTenantsByFlatId(flatId: string): Observable<TenantDto[]> {
    return this.http.get<TenantDto[]>(`${environment.apiUrl}/flats/${flatId}/tenants`);
  }

  promoteTenant(flatId: string, tenantId: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/flats/${flatId}/tenants/${tenantId}/promote`, {});
  }

  revokeOwnership(flatId: string, tenantId: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/flats/${flatId}/tenants/${tenantId}/revoke-ownership`, {});
  }

  removeTenant(flatId: string, tenantId: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/flats/${flatId}/tenants/${tenantId}`);
  }
}
