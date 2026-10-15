import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import LoginPage from './components/Auth/LoginPage';
import RegisterPage from './components/Auth/RegisterPage';
import AppLayout from './components/Layout/AppLayout';
import ProjectLayout from './components/Layout/ProjectLayout';
import ProjectList from './components/Projects/ProjectList';
import KanbanBoard from './components/Board/KanbanBoard';
import IssueList from './components/Issues/IssueList';
import IssueDetail from './components/Issues/IssueDetail';
import Dashboard from './components/Dashboard/Dashboard';
import UsersPage from './components/Auth/UsersPage';

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route element={<AppLayout />}>
            <Route path="/" element={<ProjectList />} />
            <Route path="/project/:projectId" element={<ProjectLayout />}>
              <Route path="board" element={<KanbanBoard />} />
              <Route path="issues" element={<IssueList />} />
              <Route path="issue/:issueId" element={<IssueDetail />} />
              <Route path="dashboard" element={<Dashboard />} />
            </Route>
            <Route path="/users" element={<UsersPage />} />
          </Route>
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
