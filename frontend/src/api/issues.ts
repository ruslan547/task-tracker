import api from './client';
import { Issue } from '../types';

export const getIssuesByProject = (projectId: string) =>
  api.get<Issue[]>(`/issues/project/${projectId}`);

export const getIssue = (id: string) => api.get<Issue>(`/issues/${id}`);

export const createIssue = (data: {
  title: string;
  description?: string;
  type: number;
  priority: number;
  deadline?: string;
  projectId: string;
  assigneeId?: string;
  parentIssueId?: string;
  sprintId?: string;
  tagIds?: string[];
}) => api.post<Issue>('/issues', data);

export const updateIssue = (id: string, data: Record<string, unknown>) =>
  api.put<Issue>(`/issues/${id}`, data);

export const moveIssue = (id: string, status: number) =>
  api.patch(`/issues/${id}/move`, { status });

export const deleteIssue = (id: string) => api.delete(`/issues/${id}`);
