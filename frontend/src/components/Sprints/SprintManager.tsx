import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getSprintsByProject, createSprint, startSprint, completeSprint } from '../../api/sprints';
import { useAuth } from '../../context/AuthContext';
import { Sprint } from '../../types';

const statusLabels: Record<string, string> = {
  Planning: 'Планирование',
  Active: 'Активный',
  Completed: 'Завершён',
};

const canManage = (role?: string) => role === 'Administrator' || role === 'ProjectManager';

export default function SprintManager() {
  const { user } = useAuth();
  const { projectId } = useParams<{ projectId: string }>();
  const [sprints, setSprints] = useState<Sprint[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState('');
  const [goal, setGoal] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');

  const load = () => {
    if (projectId) getSprintsByProject(projectId).then((res) => setSprints(res.data));
  };

  useEffect(() => { load(); }, [projectId]);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!projectId) return;
    await createSprint(projectId, { name, goal: goal || undefined, startDate, endDate });
    setName(''); setGoal(''); setStartDate(''); setEndDate(''); setShowForm(false);
    load();
  };

  const handleStart = async (id: string) => {
    await startSprint(id);
    load();
  };

  const handleComplete = async (id: string) => {
    if (confirm('Завершить спринт? Незавершённые задачи вернутся в бэклог.')) {
      await completeSprint(id);
      load();
    }
  };

  const activeSprint = sprints.find((s) => s.status === 'Active');

  return (
    <div className="sprint-manager">
      <div className="page-header">
        <h1>Спринты</h1>
        {canManage(user?.role) && (
          <button onClick={() => setShowForm(!showForm)} className="btn btn-primary">
            {showForm ? 'Отмена' : '+ Новый спринт'}
          </button>
        )}
      </div>

      {showForm && (
        <form onSubmit={handleCreate} className="create-form">
          <div className="form-row">
            <div className="form-group">
              <label>Название</label>
              <input value={name} onChange={(e) => setName(e.target.value)} required placeholder="Sprint 1" />
            </div>
            <div className="form-group">
              <label>Начало</label>
              <input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
            </div>
            <div className="form-group">
              <label>Конец</label>
              <input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} required />
            </div>
          </div>
          <div className="form-group">
            <label>Цель спринта</label>
            <textarea value={goal} onChange={(e) => setGoal(e.target.value)} placeholder="Что хотим достичь..." />
          </div>
          <button type="submit" className="btn btn-primary">Создать</button>
        </form>
      )}

      <div className="sprint-list">
        {sprints.map((s) => (
          <div key={s.id} className={`sprint-card ${s.status === 'Active' ? 'active' : ''}`}>
            <div className="sprint-card-header">
              <div>
                <h3>{s.name}</h3>
                <span className={`badge sprint-${s.status.toLowerCase()}`}>{statusLabels[s.status]}</span>
              </div>
              {canManage(user?.role) && (
                <div className="sprint-actions">
                  {s.status === 'Planning' && !activeSprint && (
                    <button onClick={() => handleStart(s.id)} className="btn btn-sm btn-primary">Начать</button>
                  )}
                  {s.status === 'Active' && (
                    <button onClick={() => handleComplete(s.id)} className="btn btn-sm btn-danger">Завершить</button>
                  )}
                </div>
              )}
            </div>
            {s.goal && <p className="sprint-goal">{s.goal}</p>}
            <div className="sprint-meta">
              <span>{new Date(s.startDate).toLocaleDateString('ru')} — {new Date(s.endDate).toLocaleDateString('ru')}</span>
              <span>Задач: {s.doneIssues}/{s.totalIssues}</span>
              {s.totalIssues > 0 && (
                <div className="sprint-progress">
                  <div className="sprint-progress-bar" style={{ width: `${(s.doneIssues / s.totalIssues) * 100}%` }} />
                </div>
              )}
            </div>
          </div>
        ))}
      </div>
      {sprints.length === 0 && !showForm && (
        <p className="empty-state">Нет спринтов. Создайте первый!</p>
      )}
    </div>
  );
}
