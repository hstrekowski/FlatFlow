import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  PaymentDetailDto,
  AddPaymentRequest,
  UpdatePaymentRequest,
  AddPaymentShareRequest,
} from '../../models/payment.model';
import { PaginatedResult } from '../../models/paginated-result.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  constructor(private http: HttpClient) {}

  getPaymentsByFlatId(flatId: string, page: number, pageSize: number): Observable<PaginatedResult<PaymentDetailDto>> {
    return this.http.get<PaginatedResult<PaymentDetailDto>>(
      `${environment.apiUrl}/flats/${flatId}/payments?page=${page}&pageSize=${pageSize}`,
    );
  }

  getPaymentById(flatId: string, paymentId: string): Observable<PaymentDetailDto> {
    return this.http.get<PaymentDetailDto>(`${environment.apiUrl}/flats/${flatId}/payments/${paymentId}`);
  }

  addPayment(flatId: string, request: AddPaymentRequest): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/flats/${flatId}/payments`, request);
  }

  updatePayment(flatId: string, paymentId: string, request: UpdatePaymentRequest): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/flats/${flatId}/payments/${paymentId}`, request);
  }

  removePayment(flatId: string, paymentId: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/flats/${flatId}/payments/${paymentId}`);
  }

  addShare(flatId: string, paymentId: string, request: AddPaymentShareRequest): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/flats/${flatId}/payments/${paymentId}/shares`, request);
  }

  removeShare(flatId: string, paymentId: string, shareId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.apiUrl}/flats/${flatId}/payments/${paymentId}/shares/${shareId}`,
    );
  }

  markShareAsPaid(flatId: string, paymentId: string, shareId: string): Observable<void> {
    return this.http.post<void>(
      `${environment.apiUrl}/flats/${flatId}/payments/${paymentId}/shares/${shareId}/mark-paid`,
      {},
    );
  }

  markShareAsPartial(flatId: string, paymentId: string, shareId: string): Observable<void> {
    return this.http.post<void>(
      `${environment.apiUrl}/flats/${flatId}/payments/${paymentId}/shares/${shareId}/mark-partial`,
      {},
    );
  }
}
