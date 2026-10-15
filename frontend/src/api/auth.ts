import api from './client';
import { AuthResult, User } from '../types';

export const login = (email: string, password: string) =>
  api.post<AuthResult>('/auth/login', { email, password });

export const register = (email: string, fullName: string, password: string) =>
  api.post<AuthResult>('/auth/register', { email, fullName, password });

export const getMe = () => api.get<User>('/auth/me');

export const getUsers = () => api.get<User[]>('/auth/users');
