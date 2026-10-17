import api from './client';
import { Sprint, BurndownPoint } from '../types';

export const getSprintsByProject = (projectId: string) =>
  api.get<Sprint[]>(`/sprints/project/${projectId}`);

export const createSprint = (projectId: string, data: { name: string; goal?: string; startDate: string; endDate: string }) =>
  api.post<Sprint>(`/sprints/project/${projectId}`, data);

export const startSprint = (id: string) =>
  api.patch<Sprint>(`/sprints/${id}/start`);

export const completeSprint = (id: string) =>
  api.patch<Sprint>(`/sprints/${id}/complete`);

export const assignIssueToSprint = (issueId: string, sprintId: string | null) =>
  api.patch(`/sprints/assign/${issueId}`, { sprintId });

export const getBurndown = (sprintId: string) =>
  api.get<BurndownPoint[]>(`/sprints/${sprintId}/burndown`);
