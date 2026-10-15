import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getIssuesByProject } from '../../api/issues';
import { getProject } from '../../api/projects';
import { Issue, Project } from '../../types';
import { PieChart, Pie, Cell, BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Legend } from 'recharts';

const STATUS_COLORS: Record<string, string> = {
  Todo: '#94a3b8',
  InProgress: '#3b82f6',
  Testing: '#f59e0b',
  Done: '#22c55e',
};

const STATUS_LABELS: Record<string, string> = {
  Todo: 'К выполнению',
  InProgress: 'В работе',
  Testing: 'Тестирование',
  Done: 'Готово',
};

const PRIORITY_COLORS: Record<string, string> = {
  Highest: '#ef4444',
  High: '#f97316',
  Medium: '#eab308',
  Low: '#22c55e',
  Lowest: '#94a3b8',
};

export default function Dashboard() {
  const { projectId } = useParams<{ projectId: string }>();
  const [issues, setIssues] = useState<Issue[]>([]);
  const [project, setProject] = useState<Project | null>(null);

  useEffect(() => {
    if (projectId) {
      getIssuesByProject(projectId).then((res) => setIssues(res.data));
      getProject(projectId).then((res) => setProject(res.data));
    }
  }, [projectId]);

  const statusData = Object.entries(
    issues.reduce((acc, i) => {
      acc[i.status] = (acc[i.status] || 0) + 1;
      return acc;
    }, {} as Record<string, number>)
  ).map(([name, value]) => ({ name: STATUS_LABELS[name] || name, value, color: STATUS_COLORS[name] || '#666' }));

  const priorityData = Object.entries(
    issues.reduce((acc, i) => {
      acc[i.priority] = (acc[i.priority] || 0) + 1;
      return acc;
    }, {} as Record<string, number>)
  ).map(([name, value]) => ({ name, value, color: PRIORITY_COLORS[name] || '#666' }));

  const typeData = Object.entries(
    issues.reduce((acc, i) => {
      acc[i.type] = (acc[i.type] || 0) + 1;
      return acc;
    }, {} as Record<string, number>)
  ).map(([name, value]) => ({ name, value }));

  const totalIssues = issues.length;
  const doneIssues = issues.filter((i) => i.status === 'Done').length;
  const inProgressIssues = issues.filter((i) => i.status === 'InProgress').length;

  return (
    <div className="dashboard">
      <div className="page-header">
        <h1>Дашборд: {project?.name}</h1>
        <div>
          <Link to={`/project/${projectId}/board`} className="btn btn-outline">Доска</Link>
          <Link to={`/project/${projectId}/issues`} className="btn btn-outline" style={{ marginLeft: 8 }}>Задачи</Link>
        </div>
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
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
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
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="value" name="Кол-во" fill="#6366f1" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
}
