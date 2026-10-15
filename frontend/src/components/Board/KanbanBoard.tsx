import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { DragDropContext, Droppable, Draggable, DropResult } from '@hello-pangea/dnd';
import { getBoardByProject } from '../../api/boards';
import { moveIssue } from '../../api/issues';
import { Board, BoardColumn } from '../../types';

const statusMap: Record<string, number> = {
  Todo: 0,
  InProgress: 1,
  Testing: 2,
  Done: 3,
};

const priorityColors: Record<string, string> = {
  Highest: '#ef4444',
  High: '#f97316',
  Medium: '#eab308',
  Low: '#22c55e',
  Lowest: '#94a3b8',
};

const typeIcons: Record<string, string> = {
  Bug: '\u{1F41B}',
  Task: '✅',
  Story: '\u{1F4D6}',
  Epic: '⚡',
};

export default function KanbanBoard() {
  const { projectId } = useParams<{ projectId: string }>();
  const [board, setBoard] = useState<Board | null>(null);

  const load = () => {
    if (projectId) {
      getBoardByProject(projectId).then((res) => setBoard(res.data));
    }
  };

  useEffect(() => { load(); }, [projectId]);

  const handleDragEnd = async (result: DropResult) => {
    if (!result.destination || !board) return;

    const sourceCol = board.columns.find((c) => c.id === result.source.droppableId);
    const destCol = board.columns.find((c) => c.id === result.destination!.droppableId);
    if (!sourceCol || !destCol || sourceCol.id === destCol.id) return;

    const issue = sourceCol.issues[result.source.index];
    const newStatus = statusMap[destCol.mappedStatus];

    const newColumns = board.columns.map((col): BoardColumn => {
      if (col.id === sourceCol.id) {
        return { ...col, issues: col.issues.filter((_, i) => i !== result.source.index) };
      }
      if (col.id === destCol.id) {
        const newIssues = [...col.issues];
        newIssues.splice(result.destination!.index, 0, issue);
        return { ...col, issues: newIssues };
      }
      return col;
    });
    setBoard({ ...board, columns: newColumns });

    await moveIssue(issue.id, newStatus);
  };

  if (!board) return <div className="loading">Загрузка доски...</div>;

  return (
    <div className="kanban-board">
      <div className="page-header">
        <h1>Доска: {board.name}</h1>
      </div>
      <DragDropContext onDragEnd={handleDragEnd}>
        <div className="board-columns">
          {board.columns.map((col) => (
            <div key={col.id} className="board-column">
              <div className="column-header">
                <h3>{col.name}</h3>
                <span className="column-count">{col.issues.length}</span>
              </div>
              <Droppable droppableId={col.id}>
                {(provided, snapshot) => (
                  <div
                    ref={provided.innerRef}
                    {...provided.droppableProps}
                    className={`column-body ${snapshot.isDraggingOver ? 'dragging-over' : ''}`}
                  >
                    {col.issues.map((issue, index) => (
                      <Draggable key={issue.id} draggableId={issue.id} index={index}>
                        {(provided, snapshot) => (
                          <div
                            ref={provided.innerRef}
                            {...provided.draggableProps}
                            {...provided.dragHandleProps}
                            className={`board-card ${snapshot.isDragging ? 'dragging' : ''}`}
                          >
                            <div className="card-header">
                              <span className="card-key">{issue.issueKey}</span>
                              <span className="card-priority" style={{ color: priorityColors[issue.priority] }}>
                                {issue.priority}
                              </span>
                            </div>
                            <Link to={`/project/${projectId}/issue/${issue.id}`} className="card-title">
                              {issue.title}
                            </Link>
                            <div className="card-footer">
                              <span className="card-type">{typeIcons[issue.type] || ''} {issue.type}</span>
                              {issue.assigneeName && <span className="card-assignee">{issue.assigneeName}</span>}
                            </div>
                          </div>
                        )}
                      </Draggable>
                    ))}
                    {provided.placeholder}
                  </div>
                )}
              </Droppable>
            </div>
          ))}
        </div>
      </DragDropContext>
    </div>
  );
}
