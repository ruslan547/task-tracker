import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getProjects, createProject, deleteProject } from '../../api/projects';
import { useAuth } from '../../context/AuthContext';
import { Project } from '../../types';

const canManageProjects = (role?: string) => role === 'Administrator' || role === 'ProjectManager';

export default function ProjectList() {
  const { user } = useAuth();
  const [projects, setProjects] = useState<Project[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState('');
  const [key, setKey] = useState('');
  const [description, setDescription] = useState('');
  const [error, setError] = useState('');

  const load = () => {
    getProjects().then((res) => setProjects(res.data));
  };

  useEffect(() => { load(); }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      await createProject({ name, key, description: description || undefined });
      setName(''); setKey(''); setDescription(''); setShowForm(false);
      load();
    } catch {
      setError('Не удалось создать проект.');
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm('Удалить проект?')) {
      try {
        await deleteProject(id);
        load();
      } catch {
        setError('Не удалось удалить проект.');
      }
    }
  };

  return (
    <div className="project-list">
      <div className="page-header">
        <h1>Проекты</h1>
        {canManageProjects(user?.role) && (
          <button onClick={() => setShowForm(!showForm)} className="btn btn-primary">
            {showForm ? 'Отмена' : '+ Новый проект'}
          </button>
        )}
      </div>

      {error && <div className="error-message">{error}</div>}
      {showForm && (
        <form onSubmit={handleCreate} className="create-form">
          <div className="form-row">
            <div className="form-group">
              <label>Название</label>
              <input value={name} onChange={(e) => setName(e.target.value)} required />
            </div>
            <div className="form-group">
              <label>Ключ (напр. PROJ)</label>
              <input value={key} onChange={(e) => setKey(e.target.value.toUpperCase())} required maxLength={10} />
            </div>
          </div>
          <div className="form-group">
            <label>Описание</label>
            <textarea value={description} onChange={(e) => setDescription(e.target.value)} />
          </div>
          <button type="submit" className="btn btn-primary">Создать</button>
        </form>
      )}

      <div className="project-grid">
        {projects.map((p) => (
          <div key={p.id} className="project-card">
            <div className="project-card-header">
              <Link to={`/project/${p.id}/board`}>
                <span className="project-key">{p.key}</span>
                <h3>{p.name}</h3>
              </Link>
              {canManageProjects(user?.role) && (
                <button onClick={() => handleDelete(p.id)} className="btn-icon delete" title="Удалить">&#x2715;</button>
              )}
            </div>
            {p.description && <p className="project-desc">{p.description}</p>}
            <div className="project-stats">
              <span>Задач: {p.issueCount}</span>
              <span>Участников: {p.memberCount}</span>
            </div>
            <div className="project-actions">
              <Link to={`/project/${p.id}/board`} className="btn btn-sm">Доска</Link>
              <Link to={`/project/${p.id}/issues`} className="btn btn-sm">Задачи</Link>
              <Link to={`/project/${p.id}/dashboard`} className="btn btn-sm">Дашборд</Link>
            </div>
          </div>
        ))}
      </div>
      {projects.length === 0 && !showForm && (
        <p className="empty-state">{canManageProjects(user?.role) ? 'Нет проектов. Создайте первый!' : 'Нет доступных проектов.'}</p>
      )}
    </div>
  );
}
