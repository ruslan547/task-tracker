import { NavLink, useParams } from 'react-router-dom';

export default function ProjectNav() {
  const { projectId } = useParams<{ projectId: string }>();

  return (
    <div className="project-nav">
      <NavLink to={`/project/${projectId}/board`} className={({ isActive }) => isActive ? 'project-nav-link active' : 'project-nav-link'}>
        Доска
      </NavLink>
      <NavLink to={`/project/${projectId}/issues`} className={({ isActive }) => isActive ? 'project-nav-link active' : 'project-nav-link'}>
        Задачи
      </NavLink>
      <NavLink to={`/project/${projectId}/dashboard`} className={({ isActive }) => isActive ? 'project-nav-link active' : 'project-nav-link'}>
        Дашборд
      </NavLink>
      <NavLink to="/" className="project-nav-link back">
        &larr; Все проекты
      </NavLink>
    </div>
  );
}
