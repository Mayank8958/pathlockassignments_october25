import React from 'react'
import { TaskDto } from '../services/api'

type Props = {
  task: TaskDto
  onToggle: (task: TaskDto) => void
  onDelete: (task: TaskDto) => void
}

const TaskItem: React.FC<Props> = ({ task, onToggle, onDelete }) => {
  return (
    <li className="flex items-center justify-between px-4 py-3 bg-white rounded border border-gray-200 shadow-sm">
      <div className="flex items-center gap-3">
        <input
          type="checkbox"
          checked={task.isCompleted}
          onChange={() => onToggle(task)}
          className="h-5 w-5"
        />
        <div>
          <div className={task.isCompleted ? 'line-through text-gray-500' : ''}>{task.title}</div>
          {task.description && (
            <div className="text-sm text-gray-500">{task.description}</div>
          )}
        </div>
      </div>
      <button
        onClick={() => onDelete(task)}
        className="text-red-600 hover:text-red-700 text-sm"
      >
        Delete
      </button>
    </li>
  )
}

export default TaskItem


