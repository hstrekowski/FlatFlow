export enum ChoreFrequency {
  Once = 0,
  Daily = 1,
  Weekly = 2,
  Monthly = 3,
}

export interface ChoreDto {
  id: string;
  title: string;
  description: string;
  frequency: ChoreFrequency;
  createdById: string;
}

export interface ChoreDetailDto extends ChoreDto {
  assignments: ChoreAssignmentDto[];
}

export interface ChoreAssignmentDto {
  id: string;
  tenantId: string;
  dueDate: string;
  isCompleted: boolean;
  completedAt: string | null;
}

export interface AddChoreRequest {
  flatId: string;
  title: string;
  description: string;
  frequency: ChoreFrequency;
  createdById: string;
}

export interface UpdateChoreRequest {
  choreId: string;
  title: string;
  description: string;
  frequency: ChoreFrequency;
}

export interface AddChoreAssignmentRequest {
  choreId: string;
  tenantId: string;
  dueDate: string;
}
