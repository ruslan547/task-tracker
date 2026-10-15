import api from './client';
import { Comment } from '../types';

export const getCommentsByIssue = (issueId: string) =>
  api.get<Comment[]>(`/comments/issue/${issueId}`);

export const createComment = (issueId: string, text: string) =>
  api.post<Comment>(`/comments/issue/${issueId}`, { text });
