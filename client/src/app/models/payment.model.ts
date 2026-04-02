export enum PaymentShareStatus {
  New = 0,
  Partial = 1,
  Paid = 2,
}

export interface PaymentDto {
  id: string;
  title: string;
  amount: number;
  dueDate: string;
  createdById: string;
}

export interface PaymentDetailDto extends PaymentDto {
  shares: PaymentShareDto[];
}

export interface PaymentShareDto {
  id: string;
  tenantId: string;
  shareAmount: number;
  status: PaymentShareStatus;
}

export interface AddPaymentRequest {
  flatId: string;
  title: string;
  amount: number;
  dueDate: string;
  createdById: string;
}

export interface UpdatePaymentRequest {
  paymentId: string;
  title: string;
  amount: number;
  dueDate: string;
}

export interface AddPaymentShareRequest {
  paymentId: string;
  tenantId: string;
  shareAmount: number;
}
