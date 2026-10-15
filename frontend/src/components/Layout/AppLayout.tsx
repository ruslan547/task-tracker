import { Outlet, Navigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import Navbar from './Navbar';

export default function AppLayout() {
  const { user, loading } = useAuth();

  if (loading) return <div className="loading">Загрузка...</div>;
  if (!user) return <Navigate to="/login" />;

  return (
    <div className="app-layout">
      <Navbar />
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
