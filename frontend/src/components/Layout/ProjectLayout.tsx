import { Outlet } from 'react-router-dom';
import ProjectNav from './ProjectNav';

export default function ProjectLayout() {
  return (
    <div>
      <ProjectNav />
      <Outlet />
    </div>
  );
}
