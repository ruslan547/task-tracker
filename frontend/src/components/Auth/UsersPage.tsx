import { useEffect, useState } from 'react';
import { getUsers, updateUserRole } from '../../api/auth';
import { useAuth } from '../../context/AuthContext';
import { User } from '../../types';

const roles = ['Administrator', 'ProjectManager', 'Developer', 'Tester', 'Observer'];
const roleLabels: Record<string, string> = {
  Administrator: 'Администратор',
  ProjectManager: 'Менеджер проектов',
  Developer: 'Разработчик',
  Tester: 'Тестировщик',
  Observer: 'Наблюдатель',
};

export default function UsersPage() {
  const { user: currentUser } = useAuth();
  const [users, setUsers] = useState<User[]>([]);

  const load = () => { getUsers().then((res) => setUsers(res.data)); };
  useEffect(() => { load(); }, []);

  const handleRoleChange = async (userId: string, role: string) => {
    await updateUserRole(userId, role);
    load();
  };

  if (currentUser?.role !== 'Administrator') {
    return <p className="empty-state">Доступ только для администраторов.</p>;
  }

  return (
    <div className="users-page">
      <div className="page-header">
        <h1>Управление пользователями</h1>
      </div>
      <table className="issue-table">
        <thead>
          <tr>
            <th>Имя</th>
            <th>Email</th>
            <th>Роль</th>
          </tr>
        </thead>
        <tbody>
          {users.map((u) => (
            <tr key={u.id}>
              <td>{u.fullName}</td>
              <td>{u.email}</td>
              <td>
                {u.id === currentUser.id ? (
                  <span className="badge">{roleLabels[u.role] || u.role}</span>
                ) : (
                  <select
                    value={u.role}
                    onChange={(e) => handleRoleChange(u.id, e.target.value)}
                  >
                    {roles.map((r) => (
                      <option key={r} value={r}>{roleLabels[r]}</option>
                    ))}
                  </select>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
