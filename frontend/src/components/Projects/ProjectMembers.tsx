import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getProjectMembers, addProjectMember, removeProjectMember } from '../../api/projects';
import { getUsers } from '../../api/auth';
import { useAuth } from '../../context/AuthContext';
import { ProjectMember, User } from '../../types';

const roleOptions = ['Developer', 'Tester', 'Observer', 'ProjectManager'];
const roleLabels: Record<string, string> = {
  Administrator: 'Администратор',
  ProjectManager: 'Менеджер',
  Developer: 'Разработчик',
  Tester: 'Тестировщик',
  Observer: 'Наблюдатель',
};

const canManage = (role?: string) => role === 'Administrator' || role === 'ProjectManager';

export default function ProjectMembers() {
  const { user } = useAuth();
  const { projectId } = useParams<{ projectId: string }>();
  const [members, setMembers] = useState<ProjectMember[]>([]);
  const [allUsers, setAllUsers] = useState<User[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [selectedUserId, setSelectedUserId] = useState('');
  const [selectedRole, setSelectedRole] = useState('Developer');
  const [error, setError] = useState('');

  const load = () => {
    if (projectId) getProjectMembers(projectId).then((res) => setMembers(res.data));
  };

  useEffect(() => {
    load();
    getUsers().then((res) => setAllUsers(res.data));
  }, [projectId]);

  const availableUsers = allUsers.filter((u) => !members.some((m) => m.userId === u.id));

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!projectId || !selectedUserId) return;
    setError('');
    try {
      await addProjectMember(projectId, selectedUserId, selectedRole);
      setSelectedUserId('');
      setShowForm(false);
      load();
    } catch {
      setError('Не удалось добавить участника.');
    }
  };

  const handleRemove = async (userId: string) => {
    if (!projectId || !confirm('Удалить участника из проекта?')) return;
    try {
      await removeProjectMember(projectId, userId);
      load();
    } catch {
      setError('Не удалось удалить участника.');
    }
  };

  return (
    <div className="issue-list">
      {error && <div className="error-message">{error}</div>}
      <div className="page-header">
        <h1>Участники</h1>
        {canManage(user?.role) && (
          <button onClick={() => setShowForm(!showForm)} className="btn btn-primary">
            {showForm ? 'Отмена' : '+ Добавить'}
          </button>
        )}
      </div>

      {showForm && (
        <form onSubmit={handleAdd} className="create-form">
          <div className="form-row">
            <div className="form-group">
              <label>Пользователь</label>
              <select value={selectedUserId} onChange={(e) => setSelectedUserId(e.target.value)} required>
                <option value="">Выберите...</option>
                {availableUsers.map((u) => (
                  <option key={u.id} value={u.id}>{u.fullName} ({u.email})</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Роль в проекте</label>
              <select value={selectedRole} onChange={(e) => setSelectedRole(e.target.value)}>
                {roleOptions.map((r) => (
                  <option key={r} value={r}>{roleLabels[r]}</option>
                ))}
              </select>
            </div>
          </div>
          <button type="submit" className="btn btn-primary">Добавить</button>
        </form>
      )}

      <table className="issue-table">
        <thead>
          <tr>
            <th>Имя</th>
            <th>Email</th>
            <th>Роль</th>
            {canManage(user?.role) && <th>Действия</th>}
          </tr>
        </thead>
        <tbody>
          {members.map((m) => (
            <tr key={m.userId}>
              <td>{m.fullName}</td>
              <td>{m.email}</td>
              <td><span className="badge">{roleLabels[m.role] || m.role}</span></td>
              {canManage(user?.role) && (
                <td>
                  <button onClick={() => handleRemove(m.userId)} className="btn btn-sm btn-danger">Удалить</button>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
      {members.length === 0 && <p className="empty-state">Нет участников.</p>}
    </div>
  );
}
