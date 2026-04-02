export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  userId: string;
  email: string;
  token: string;
}

export interface ProfileDto {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
}
