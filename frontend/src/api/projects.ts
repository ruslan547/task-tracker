import api from './client';
import { Project, ProjectMember } from '../types';

export const getProjects = () => api.get<Project[]>('/projects');

export const getProject = (id: string) => api.get<Project>(`/projects/${id}`);

export const createProject = (data: { name: string; key: string; description?: string }) =>
  api.post<Project>('/projects', data);

export const updateProject = (id: string, data: { name: string; description?: string }) =>
  api.put<Project>(`/projects/${id}`, data);

export const deleteProject = (id: string) => api.delete(`/projects/${id}`);

export const getProjectMembers = (projectId: string) =>
  api.get<ProjectMember[]>(`/projects/${projectId}/members`);

export const addProjectMember = (projectId: string, userId: string, role: string) =>
  api.post(`/projects/${projectId}/members`, { userId, role });

export const removeProjectMember = (projectId: string, userId: string) =>
  api.delete(`/projects/${projectId}/members/${userId}`);
