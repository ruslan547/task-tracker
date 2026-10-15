export interface AuthResult {
  token: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
}

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: string;
}

export interface Project {
  id: string;
  name: string;
  key: string;
  description?: string;
  ownerId: string;
  ownerName: string;
  createdAt: string;
  memberCount: number;
  issueCount: number;
}

export type IssueType = 'Bug' | 'Task' | 'Story' | 'Epic';
export type IssuePriority = 'Lowest' | 'Low' | 'Medium' | 'High' | 'Highest';
export type IssueStatus = 'Todo' | 'InProgress' | 'Testing' | 'Done';

export interface Issue {
  id: string;
  title: string;
  description?: string;
  type: IssueType;
  priority: IssuePriority;
  status: IssueStatus;
  issueKey: string;
  issueNumber: number;
  deadline?: string;
  projectId: string;
  assigneeId?: string;
  assigneeName?: string;
  reporterId: string;
  reporterName: string;
  parentIssueId?: string;
  sprintId?: string;
  createdAt: string;
  updatedAt?: string;
  tags: string[];
}

export interface BoardColumn {
  id: string;
  name: string;
  order: number;
  mappedStatus: string;
  issues: BoardIssue[];
}

export interface BoardIssue {
  id: string;
  title: string;
  issueKey: string;
  type: string;
  priority: string;
  assigneeName?: string;
  assigneeId?: string;
}

export interface Board {
  id: string;
  name: string;
  projectId: string;
  columns: BoardColumn[];
}

export type SprintStatus = 'Planning' | 'Active' | 'Completed';

export interface Sprint {
  id: string;
  name: string;
  goal?: string;
  startDate: string;
  endDate: string;
  status: SprintStatus;
  projectId: string;
  totalIssues: number;
  doneIssues: number;
}

export interface BurndownPoint {
  date: string;
  remaining: number;
}

export interface Comment {
  id: string;
  text: string;
  authorId: string;
  authorName: string;
  createdAt: string;
}
