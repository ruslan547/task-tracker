import api from './client';
import { Board } from '../types';

export const getBoardByProject = (projectId: string) =>
  api.get<Board>(`/boards/project/${projectId}`);
