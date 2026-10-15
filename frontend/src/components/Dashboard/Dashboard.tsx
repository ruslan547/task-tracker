import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getIssuesByProject } from '../../api/issues';
import { getProject } from '../../api/projects';
import { getSprintsByProject, getBurndown } from '../../api/sprints';
import { Issue, Project, Sprint, BurndownPoint } from '../../types';
import {
  PieChart, Pie, Cell, BarChart, Bar, XAxis, YAxis, Tooltip,
  ResponsiveContainer, Legend, LineChart, Line, CartesianGrid
} from 'recharts';

const STATUS_COLORS: Record<string, string> = {
  Todo: '#94a3b8', InProgress: '#3b82f6', Testing: '#f59e0b', Done: '#22c55e',
};
const STATUS_LABELS: Record<string, string> = {
  Todo: 'К выполнению', InProgress: 'В работе', Testing: 'Тестирование', Done: 'Готово',
};
const PRIORITY_COLORS: Record<string, string> = {
  Highest: '#ef4444', High: '#f97316', Medium: '#eab308', Low: '#22c55e', Lowest: '#94a3b8',
};

export default function Dashboard() {
  const { projectId } = useParams<{ projectId: string }>();
  const [issues, setIssues] = useState<Issue[]>([]);
  const [project, setProject] = useState<Project | null>(null);
  const [sprints, setSprints] = useState<Sprint[]>([]);
  const [burndown, setBurndown] = useState<BurndownPoint[]>([]);
  const [selectedSprintId, setSelectedSprintId] = useState<string>('');

  useEffect(() => {
    if (projectId) {
      getIssuesByProject(projectId).then((res) => setIssues(res.data));
      getProject(projectId).then((res) => setProject(res.data));
      getSprintsByProject(projectId).then((res) => {
        setSprints(res.data);
        const active = res.data.find((s) => s.status === 'Active');
        if (active) setSelectedSprintId(active.id);
      });
    }
  }, [projectId]);

  useEffect(() => {
    if (selectedSprintId) {
      getBurndown(selectedSprintId).then((res) => setBurndown(res.data));
    } else {
      setBurndown([]);
    }
  }, [selectedSprintId]);

  const statusData = Object.entries(
    issues.reduce((acc, i) => { acc[i.status] = (acc[i.status] || 0) + 1; return acc; }, {} as Record<string, number>)
  ).map(([name, value]) => ({ name: STATUS_LABELS[name] || name, value, color: STATUS_COLORS[name] || '#666' }));

  const priorityData = Object.entries(
    issues.reduce((acc, i) => { acc[i.priority] = (acc[i.priority] || 0) + 1; return acc; }, {} as Record<string, number>)
  ).map(([name, value]) => ({ name, value, color: PRIORITY_COLORS[name] || '#666' }));

  const typeData = Object.entries(
    issues.reduce((acc, i) => { acc[i.type] = (acc[i.type] || 0) + 1; return acc; }, {} as Record<string, number>)
  ).map(([name, value]) => ({ name, value }));

  const totalIssues = issues.length;
  const doneIssues = issues.filter((i) => i.status === 'Done').length;
  const inProgressIssues = issues.filter((i) => i.status === 'InProgress').length;

  const burndownData = burndown.map((p) => ({
    date: new Date(p.date).toLocaleDateString('ru', { day: '2-digit', month: '2-digit' }),
    remaining: p.remaining,
  }));

  return (
    <div className="dashboard">
      <div className="page-header">
        <h1>Дашборд: {project?.name}</h1>
      </div>

      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-value">{totalIssues}</div>
          <div className="stat-label">Всего задач</div>
        </div>
        <div className="stat-card">
          <div className="stat-value">{doneIssues}</div>
          <div className="stat-label">Выполнено</div>
        </div>
        <div className="stat-card">
          <div className="stat-value">{inProgressIssues}</div>
          <div className="stat-label">В работе</div>
        </div>
        <div className="stat-card">
          <div className="stat-value">{totalIssues > 0 ? Math.round((doneIssues / totalIssues) * 100) : 0}%</div>
          <div className="stat-label">Прогресс</div>
        </div>
      </div>

      {sprints.length > 0 && (
        <div className="chart-card" style={{ marginBottom: 16 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
            <h3>Диаграмма сгорания (Burndown)</h3>
            <select value={selectedSprintId} onChange={(e) => setSelectedSprintId(e.target.value)} style={{ padding: '6px 10px', borderRadius: 6, border: '1px solid #e2e8f0' }}>
              <option value="">Выберите спринт</option>
              {sprints.map((s) => <option key={s.id} value={s.id}>{s.name}</option>)}
            </select>
          </div>
          {burndownData.length > 0 ? (
            <ResponsiveContainer width="100%" height={250}>
              <LineChart data={burndownData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis />
                <Tooltip />
                <Line type="monotone" dataKey="remaining" name="Осталось задач" stroke="#6366f1" strokeWidth={2} dot={{ r: 3 }} />
              </LineChart>
            </ResponsiveContainer>
          ) : (
            <p style={{ color: '#64748b', textAlign: 'center', padding: 20 }}>Выберите спринт для отображения графика</p>
          )}
        </div>
      )}

      <div className="charts-grid">
        <div className="chart-card">
          <h3>По статусу</h3>
          <ResponsiveContainer width="100%" height={250}>
            <PieChart>
              <Pie data={statusData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={80} label>
                {statusData.map((entry, i) => <Cell key={i} fill={entry.color} />)}
              </Pie>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>
        <div className="chart-card">
          <h3>По приоритету</h3>
          <ResponsiveContainer width="100%" height={250}>
            <BarChart data={priorityData}>
              <XAxis dataKey="name" /><YAxis /><Tooltip />
              <Bar dataKey="value" name="Кол-во">
                {priorityData.map((entry, i) => <Cell key={i} fill={entry.color} />)}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>
        <div className="chart-card">
          <h3>По типу</h3>
          <ResponsiveContainer width="100%" height={250}>
            <BarChart data={typeData}>
              <XAxis dataKey="name" /><YAxis /><Tooltip />
              <Bar dataKey="value" name="Кол-во" fill="#6366f1" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
}
