import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { register } from '../../api/auth';
import { useAuth } from '../../context/AuthContext';

export default function RegisterPage() {
  const [email, setEmail] = useState('');
  const [fullName, setFullName] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { setAuth } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      const res = await register(email, fullName, password);
      setAuth(res.data.token, {
        id: res.data.userId,
        email: res.data.email,
        fullName: res.data.fullName,
        role: res.data.role,
      });
      navigate('/');
    } catch {
      setError('Ошибка регистрации. Проверьте данные.');
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h1>Регистрация</h1>
        {error && <div className="error-message">{error}</div>}
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Полное имя</label>
            <input type="text" value={fullName} onChange={(e) => setFullName(e.target.value)} required />
          </div>
          <div className="form-group">
            <label>Email</label>
            <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          <div className="form-group">
            <label>Пароль</label>
            <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required minLength={6} />
          </div>
          <button type="submit" className="btn btn-primary">Зарегистрироваться</button>
        </form>
        <p className="auth-link">Уже есть аккаунт? <Link to="/login">Войти</Link></p>
      </div>
    </div>
  );
}
