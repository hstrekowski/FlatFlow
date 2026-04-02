export interface FlatDto {
  id: string;
  name: string;
  street: string;
  city: string;
  zipCode: string;
  country: string;
}

export interface FlatDetailDto extends FlatDto {
  accessCode: string;
  tenants: TenantDto[];
  chores: ChoreDto[];
  payments: PaymentDto[];
  notes: NoteDto[];
}

export interface CreateFlatRequest {
  name: string;
  street: string;
  city: string;
  zipCode: string;
  country: string;
}

export interface UpdateFlatRequest {
  flatId: string;
  name: string;
  street: string;
  city: string;
  zipCode: string;
  country: string;
}

export interface JoinFlatRequest {
  accessCode: string;
}

import { TenantDto } from './tenant.model';
import { ChoreDto } from './chore.model';
import { PaymentDto } from './payment.model';
import { NoteDto } from './note.model';
