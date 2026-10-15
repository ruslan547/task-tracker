import api from './client';
import { Project } from '../types';

export const getProjects = () => api.get<Project[]>('/projects');

export const getProject = (id: string) => api.get<Project>(`/projects/${id}`);

export const createProject = (data: { name: string; key: string; description?: string }) =>
  api.post<Project>('/projects', data);

export const updateProject = (id: string, data: { name: string; description?: string }) =>
  api.put<Project>(`/projects/${id}`, data);

export const deleteProject = (id: string) => api.delete(`/projects/${id}`);
