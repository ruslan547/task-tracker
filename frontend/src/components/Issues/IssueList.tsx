import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getIssuesByProject, createIssue } from '../../api/issues';
import { getUsers } from '../../api/auth';
import { getSprintsByProject } from '../../api/sprints';
import { useAuth } from '../../context/AuthContext';
import { Issue, User, Sprint } from '../../types';
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
  const [sprintId, setSprintId] = useState('');
  const [sprints, setSprints] = useState<Sprint[]>([]);
  const [error, setError] = useState('');
  const [filterSprint, setFilterSprint] = useState('all');

  const load = () => {
    if (projectId) {
      getIssuesByProject(projectId).then((res) => setIssues(res.data));
      getProject(projectId).then((res) => setProjectKey(res.data.key));
    }
  };

  useEffect(() => {
    load();
    getUsers().then((res) => setUsers(res.data));
    if (projectId) getSprintsByProject(projectId).then((res) => setSprints(res.data));
  }, [projectId]);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!projectId) return;
    setError('');
    try {
      await createIssue({
        title,
        description: description || undefined,
        type,
        priority,
        projectId,
        assigneeId: assigneeId || undefined,
        sprintId: sprintId || undefined,
      });
      setTitle(''); setDescription(''); setSprintId(''); setShowForm(false);
      load();
    } catch {
      setError('Не удалось создать задачу.');
    }
  };

  const filteredIssues = issues.filter((issue) => {
    if (filterSprint === 'all') return true;
    if (filterSprint === 'backlog') return !issue.sprintId;
    return issue.sprintId === filterSprint;
  });

  return (
    <div className="issue-list">
      {error && <div className="error-message">{error}</div>}
      <div className="page-header">
        <h1>Задачи ({projectKey})</h1>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <select
            className="sprint-select"
            value={filterSprint}
            onChange={(e) => setFilterSprint(e.target.value)}
          >
            <option value="all">Все задачи</option>
            <option value="backlog">Бэклог</option>
            {sprints.map((s) => (
              <option key={s.id} value={s.id}>
                {s.name} {s.status === 'Active' ? '(активный)' : s.status === 'Completed' ? '(завершён)' : '(план.)'}
              </option>
            ))}
          </select>
          {canCreateIssues(user?.role) && (
            <button onClick={() => setShowForm(!showForm)} className="btn btn-primary">
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
            <div className="form-group">
              <label>Спринт</label>
              <select value={sprintId} onChange={(e) => setSprintId(e.target.value)}>
                <option value="">Бэклог</option>
                {sprints.filter((s) => s.status !== 'Completed').map((s) => (
                  <option key={s.id} value={s.id}>{s.name}</option>
                ))}
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
            <th>Спринт</th>
            <th>Исполнитель</th>
          </tr>
        </thead>
        <tbody>
          {filteredIssues.map((issue) => (
            <tr key={issue.id}>
              <td><Link to={`/project/${projectId}/issue/${issue.id}`}>{issue.issueKey}</Link></td>
              <td><Link to={`/project/${projectId}/issue/${issue.id}`}>{issue.title}</Link></td>
              <td><span className={`badge type-${toStr(issue.type, typeOptions).toLowerCase()}`}>{toStr(issue.type, typeOptions)}</span></td>
              <td><span className={`badge priority-${toStr(issue.priority, priorityOptions).toLowerCase()}`}>{toStr(issue.priority, priorityOptions)}</span></td>
              <td><span className={`badge status-${toStr(issue.status, statusOptions).toLowerCase()}`}>{statusLabels[toStr(issue.status, statusOptions)] || toStr(issue.status, statusOptions)}</span></td>
              <td>{sprints.find((s) => s.id === issue.sprintId)?.name || 'Бэклог'}</td>
              <td>{issue.assigneeName || '—'}</td>
            </tr>
          ))}
        </tbody>
      </table>
      {filteredIssues.length === 0 && !showForm && (
        <p className="empty-state">{filterSprint === 'all' ? 'Нет задач в проекте. Создайте первую!' : filterSprint === 'backlog' ? 'Бэклог пуст.' : 'В этом спринте нет задач.'}</p>
      )}
    </div>
  );
}
