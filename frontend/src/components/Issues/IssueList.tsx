import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getIssuesByProject, createIssue } from '../../api/issues';
import { getUsers } from '../../api/auth';
import { useAuth } from '../../context/AuthContext';
import { Issue, User } from '../../types';
import { getProject } from '../../api/projects';

const canCreateIssues = (role?: string) => role !== 'Observer';

const typeOptions = ['Bug', 'Task', 'Story', 'Epic'];
const priorityOptions = ['Lowest', 'Low', 'Medium', 'High', 'Highest'];
const statusOptions = ['Todo', 'InProgress', 'Testing', 'Done'];
const statusLabels: Record<string, string> = {
  Todo: 'К выполнению',
  InProgress: 'В работе',
  Testing: 'Тестирование',
  Done: 'Готово',
};

const toStr = (val: string | number, options: string[]) =>
  typeof val === 'number' ? options[val] ?? String(val) : val;

export default function IssueList() {
  const { user } = useAuth();
  const { projectId } = useParams<{ projectId: string }>();
  const [issues, setIssues] = useState<Issue[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [projectKey, setProjectKey] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [type, setType] = useState(1);
  const [priority, setPriority] = useState(2);
  const [assigneeId, setAssigneeId] = useState('');

  const load = () => {
    if (projectId) {
      getIssuesByProject(projectId).then((res) => setIssues(res.data));
      getProject(projectId).then((res) => setProjectKey(res.data.key));
    }
  };

  useEffect(() => {
    load();
    getUsers().then((res) => setUsers(res.data));
  }, [projectId]);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!projectId) return;
    await createIssue({
      title,
      description: description || undefined,
      type,
      priority,
      projectId,
      assigneeId: assigneeId || undefined,
    });
    setTitle(''); setDescription(''); setShowForm(false);
    load();
  };

  return (
    <div className="issue-list">
      <div className="page-header">
        <h1>Задачи ({projectKey})</h1>
        <div>
          {canCreateIssues(user?.role) && (
            <button onClick={() => setShowForm(!showForm)} className="btn btn-primary" style={{ marginLeft: 8 }}>
              {showForm ? 'Отмена' : '+ Новая задача'}
            </button>
          )}
        </div>
      </div>

      {showForm && (
        <form onSubmit={handleCreate} className="create-form">
          <div className="form-row">
            <div className="form-group">
              <label>Название</label>
              <input value={title} onChange={(e) => setTitle(e.target.value)} required />
            </div>
            <div className="form-group">
              <label>Тип</label>
              <select value={type} onChange={(e) => setType(Number(e.target.value))}>
                {typeOptions.map((t, i) => <option key={t} value={i}>{t}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Приоритет</label>
              <select value={priority} onChange={(e) => setPriority(Number(e.target.value))}>
                {priorityOptions.map((p, i) => <option key={p} value={i}>{p}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Исполнитель</label>
              <select value={assigneeId} onChange={(e) => setAssigneeId(e.target.value)}>
                <option value="">Не назначен</option>
                {users.map((u) => <option key={u.id} value={u.id}>{u.fullName}</option>)}
              </select>
            </div>
          </div>
          <div className="form-group">
            <label>Описание</label>
            <textarea value={description} onChange={(e) => setDescription(e.target.value)} />
          </div>
          <button type="submit" className="btn btn-primary">Создать</button>
        </form>
      )}

      <table className="issue-table">
        <thead>
          <tr>
            <th>Ключ</th>
            <th>Название</th>
            <th>Тип</th>
            <th>Приоритет</th>
            <th>Статус</th>
            <th>Исполнитель</th>
          </tr>
        </thead>
        <tbody>
          {issues.map((issue) => (
            <tr key={issue.id}>
              <td><Link to={`/project/${projectId}/issue/${issue.id}`}>{issue.issueKey}</Link></td>
              <td><Link to={`/project/${projectId}/issue/${issue.id}`}>{issue.title}</Link></td>
              <td><span className={`badge type-${toStr(issue.type, typeOptions).toLowerCase()}`}>{toStr(issue.type, typeOptions)}</span></td>
              <td><span className={`badge priority-${toStr(issue.priority, priorityOptions).toLowerCase()}`}>{toStr(issue.priority, priorityOptions)}</span></td>
              <td><span className={`badge status-${toStr(issue.status, statusOptions).toLowerCase()}`}>{statusLabels[toStr(issue.status, statusOptions)] || toStr(issue.status, statusOptions)}</span></td>
              <td>{issue.assigneeName || '—'}</td>
            </tr>
          ))}
        </tbody>
      </table>
      {issues.length === 0 && !showForm && (
        <p className="empty-state">Нет задач в проекте. Создайте первую!</p>
      )}
    </div>
  );
}
