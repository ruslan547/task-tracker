import api from './client';
import { AuthResult, User } from '../types';

export const login = (email: string, password: string) =>
  api.post<AuthResult>('/auth/login', { email, password });

export const register = (email: string, fullName: string, password: string) =>
  api.post<AuthResult>('/auth/register', { email, fullName, password });

export const getMe = () => api.get<User>('/auth/me');

export const getUsers = () => api.get<User[]>('/auth/users');

export const updateUserRole = (userId: string, role: string) =>
  api.patch<User>(`/auth/users/${userId}/role`, { role });
