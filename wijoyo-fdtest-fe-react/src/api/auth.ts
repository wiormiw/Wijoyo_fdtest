import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
});

export interface AuthResponse {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
}

export interface UserProfile {
  userName: string;
  email: string;
  emailStatus: string;
}

export async function login(payload: { email: string; password: string }): Promise<AuthResponse> {
  const { data } = await api.post('/Auth/login', payload);
  return data;
}

export async function register(payload: { email: string; password: string; userName: string }): Promise<AuthResponse> {
  const { data } = await api.post('/Auth/register', payload);
  return data;
}

export async function getProfile(token: string): Promise<UserProfile> {
  const { data } = await api.get('Users/me', {
    headers: { Authorization: `Bearer ${token}` },
  });
  return data;
}

export async function changePassword(payload: { currentPassword: string; newPassword: string }, token: string): Promise<void> {
  await api.put('/Auth/change-password', payload, {
    headers: { Authorization: `Bearer ${token}` },
  });
}

export async function sendNewPassword(email: string): Promise<void> {
  await api.post('/Auth/send-new-password', { email });
}