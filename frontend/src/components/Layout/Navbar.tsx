import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <Link to="/">TaskTracker</Link>
      </div>
      <div className="navbar-menu">
        <Link to="/">Проекты</Link>
        {user && (
          <div className="navbar-user">
            <span>{user.fullName} ({user.role})</span>
            <button onClick={handleLogout} className="btn btn-outline">Выйти</button>
          </div>
        )}
      </div>
    </nav>
  );
}
