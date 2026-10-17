import { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { getIssue, updateIssue, deleteIssue } from '../../api/issues';
import { getCommentsByIssue, createComment } from '../../api/comments';
import { getUsers } from '../../api/auth';
import { getSprintsByProject, assignIssueToSprint } from '../../api/sprints';
import { useAuth } from '../../context/AuthContext';
import { Issue, Comment, User, Sprint } from '../../types';

const statusOptions = [
  { value: 0, label: 'К выполнению' },
  { value: 1, label: 'В работе' },
  { value: 2, label: 'Тестирование' },
  { value: 3, label: 'Готово' },
];

const priorityOptions = ['Lowest', 'Low', 'Medium', 'High', 'Highest'];

export default function IssueDetail() {
  const { user: currentUser } = useAuth();
  const { projectId, issueId } = useParams<{ projectId: string; issueId: string }>();
  const navigate = useNavigate();
  const isObserver = currentUser?.role === 'Observer';
  const canDelete = currentUser?.role === 'Administrator' || currentUser?.role === 'ProjectManager';
  const [issue, setIssue] = useState<Issue | null>(null);
  const [comments, setComments] = useState<Comment[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [newComment, setNewComment] = useState('');
  const [sprints, setSprints] = useState<Sprint[]>([]);
  const [editing, setEditing] = useState(false);
  const [editTitle, setEditTitle] = useState('');
  const [editDescription, setEditDescription] = useState('');

  useEffect(() => {
    if (issueId) {
      getIssue(issueId).then((res) => {
        setIssue(res.data);
        setEditTitle(res.data.title);
        setEditDescription(res.data.description || '');
      });
      getCommentsByIssue(issueId).then((res) => setComments(res.data));
    }
    getUsers().then((res) => setUsers(res.data));
    if (projectId) getSprintsByProject(projectId).then((res) => setSprints(res.data));
  }, [issueId, projectId]);

  const handleStatusChange = async (status: number) => {
    if (!issueId) return;
    const res = await updateIssue(issueId, { status });
    setIssue(res.data);
  };

  const handlePriorityChange = async (priority: number) => {
    if (!issueId) return;
    const res = await updateIssue(issueId, { priority });
    setIssue(res.data);
  };

  const handleAssigneeChange = async (assigneeId: string) => {
    if (!issueId) return;
    const res = await updateIssue(issueId, { assigneeId: assigneeId || null });
    setIssue(res.data);
  };

  const handleSprintChange = async (sprintId: string) => {
    if (!issueId) return;
    await assignIssueToSprint(issueId, sprintId || null);
    setIssue((prev) => prev ? { ...prev, sprintId: sprintId || undefined } : prev);
  };

  const handleSaveEdit = async () => {
    if (!issueId) return;
    const res = await updateIssue(issueId, { title: editTitle, description: editDescription });
    setIssue(res.data);
    setEditing(false);
  };

  const handleDelete = async () => {
    if (!issueId || !confirm('Удалить задачу?')) return;
    await deleteIssue(issueId);
    navigate(`/project/${projectId}/issues`);
  };

  const handleAddComment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!issueId || !newComment.trim()) return;
    const res = await createComment(issueId, newComment);
    setComments([...comments, res.data]);
    setNewComment('');
  };

  if (!issue) return <div className="loading">Загрузка...</div>;

  return (
    <div className="issue-detail">
      <div className="page-header">
        <div>
          <Link to={`/project/${projectId}/issues`} className="back-link">&larr; К списку задач</Link>
          <span className="issue-key-badge">{issue.issueKey}</span>
        </div>
        {canDelete && <button onClick={handleDelete} className="btn btn-danger">Удалить</button>}
      </div>

      <div className="issue-content">
        <div className="issue-main">
          {editing ? (
            <div className="edit-form">
              <input className="edit-title" value={editTitle} onChange={(e) => setEditTitle(e.target.value)} />
              <textarea className="edit-desc" value={editDescription} onChange={(e) => setEditDescription(e.target.value)} />
              <div>
                <button onClick={handleSaveEdit} className="btn btn-primary">Сохранить</button>
                <button onClick={() => setEditing(false)} className="btn btn-outline" style={{ marginLeft: 8 }}>Отмена</button>
              </div>
            </div>
          ) : (
            <div onClick={() => !isObserver && setEditing(true)} style={{ cursor: isObserver ? 'default' : 'pointer' }}>
              <h1>{issue.title}</h1>
              <p className="issue-description">{issue.description || (isObserver ? 'Нет описания.' : 'Нет описания. Нажмите для редактирования.')}</p>
            </div>
          )}

          <div className="comments-section">
            <h2>Комментарии ({comments.length})</h2>
            {comments.map((c) => (
              <div key={c.id} className="comment">
                <div className="comment-header">
                  <strong>{c.authorName}</strong>
                  <span className="comment-date">{new Date(c.createdAt).toLocaleString('ru')}</span>
                </div>
                <p>{c.text}</p>
              </div>
            ))}
            {!isObserver && (
              <form onSubmit={handleAddComment} className="comment-form">
                <textarea
                  value={newComment}
                  onChange={(e) => setNewComment(e.target.value)}
                  placeholder="Добавить комментарий..."
                />
                <button type="submit" className="btn btn-primary" disabled={!newComment.trim()}>
                  Отправить
                </button>
              </form>
            )}
          </div>
        </div>

        <div className="issue-sidebar">
          <div className="sidebar-field">
            <label>Статус</label>
            {isObserver ? (
              <span className={`badge status-${issue.status.toLowerCase()}`}>{statusOptions.find(s => s.label === (
                issue.status === 'Todo' ? 'К выполнению' :
                issue.status === 'InProgress' ? 'В работе' :
                issue.status === 'Testing' ? 'Тестирование' : 'Готово'
              ))?.label}</span>
            ) : (
              <select
                value={statusOptions.findIndex(s => s.label === (
                  issue.status === 'Todo' ? 'К выполнению' :
                  issue.status === 'InProgress' ? 'В работе' :
                  issue.status === 'Testing' ? 'Тестирование' : 'Готово'
                ))}
                onChange={(e) => handleStatusChange(Number(e.target.value))}
              >
                {statusOptions.map((s) => <option key={s.value} value={s.value}>{s.label}</option>)}
              </select>
            )}
          </div>
          <div className="sidebar-field">
            <label>Приоритет</label>
            {isObserver ? (
              <span className={`badge priority-${issue.priority.toLowerCase()}`}>{issue.priority}</span>
            ) : (
              <select
                value={priorityOptions.indexOf(issue.priority)}
                onChange={(e) => handlePriorityChange(Number(e.target.value))}
              >
                {priorityOptions.map((p, i) => <option key={p} value={i}>{p}</option>)}
              </select>
            )}
          </div>
          <div className="sidebar-field">
            <label>Исполнитель</label>
            {isObserver ? (
              <span>{issue.assigneeName || 'Не назначен'}</span>
            ) : (
            <select
              value={issue.assigneeId || ''}
              onChange={(e) => handleAssigneeChange(e.target.value)}
            >
              <option value="">Не назначен</option>
              {users.map((u) => <option key={u.id} value={u.id}>{u.fullName}</option>)}
            </select>
            )}
          </div>
          <div className="sidebar-field">
            <label>Спринт</label>
            {isObserver ? (
              <span>{sprints.find((s) => s.id === issue.sprintId)?.name || 'Бэклог'}</span>
            ) : (
              <select
                value={issue.sprintId || ''}
                onChange={(e) => handleSprintChange(e.target.value)}
              >
                <option value="">Бэклог</option>
                {sprints.filter((s) => s.status !== 'Completed').map((s) => (
                  <option key={s.id} value={s.id}>{s.name}</option>
                ))}
              </select>
            )}
          </div>
          <div className="sidebar-field">
            <label>Тип</label>
            <span className={`badge type-${issue.type.toLowerCase()}`}>{issue.type}</span>
          </div>
          <div className="sidebar-field">
            <label>Автор</label>
            <span>{issue.reporterName}</span>
          </div>
          <div className="sidebar-field">
            <label>Создана</label>
            <span>{new Date(issue.createdAt).toLocaleString('ru')}</span>
          </div>
          {issue.updatedAt && (
            <div className="sidebar-field">
              <label>Обновлена</label>
              <span>{new Date(issue.updatedAt).toLocaleString('ru')}</span>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
