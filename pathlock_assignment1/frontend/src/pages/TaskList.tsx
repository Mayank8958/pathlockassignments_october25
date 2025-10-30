import React, { useEffect, useMemo, useState } from 'react'
import TaskItem from '../components/TaskItem'
import TaskForm from '../components/TaskForm'
import {
  TaskDto,
  getTasks,
  createTask,
  updateTask,
  deleteTask,
} from '../services/api'

type Filter = 'all' | 'active' | 'completed'

const LOCAL_KEY_TASKS = 'taskmanager_tasks_cache'
const LOCAL_KEY_FILTER = 'taskmanager_filter'

const TaskList: React.FC = () => {
  const [tasks, setTasks] = useState<TaskDto[]>(() => {
    const cached = localStorage.getItem(LOCAL_KEY_TASKS)
    return cached ? JSON.parse(cached) as TaskDto[] : []
  })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [filter, setFilter] = useState<Filter>(() => (localStorage.getItem(LOCAL_KEY_FILTER) as Filter) || 'all')

  useEffect(() => {
    const load = async () => {
      setLoading(true)
      setError(null)
      try {
        const data = await getTasks()
        setTasks(data)
        localStorage.setItem(LOCAL_KEY_TASKS, JSON.stringify(data))
      } catch (err: any) {
        setError(err?.message ?? 'Failed to load tasks')
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [])

  useEffect(() => {
    localStorage.setItem(LOCAL_KEY_FILTER, filter)
  }, [filter])

  const filtered = useMemo(() => {
    switch (filter) {
      case 'active':
        return tasks.filter(t => !t.isCompleted)
      case 'completed':
        return tasks.filter(t => t.isCompleted)
      default:
        return tasks
    }
  }, [tasks, filter])

  const handleAdd = async (title: string, description?: string) => {
    const created = await createTask({ title, description })
    const next = [created, ...tasks]
    setTasks(next)
    localStorage.setItem(LOCAL_KEY_TASKS, JSON.stringify(next))
  }

  const handleToggle = async (task: TaskDto) => {
    await updateTask(task.id, { title: task.title, description: task.description ?? undefined, isCompleted: !task.isCompleted })
    const next = tasks.map(t => t.id === task.id ? { ...t, isCompleted: !t.isCompleted } : t)
    setTasks(next)
    localStorage.setItem(LOCAL_KEY_TASKS, JSON.stringify(next))
  }

  const handleDelete = async (task: TaskDto) => {
    await deleteTask(task.id)
    const next = tasks.filter(t => t.id !== task.id)
    setTasks(next)
    localStorage.setItem(LOCAL_KEY_TASKS, JSON.stringify(next))
  }

  return (
    <div className="max-w-2xl mx-auto p-4">
      <h1 className="text-2xl font-semibold mb-4">Basic Task Manager</h1>
      <div className="bg-white p-4 rounded border border-gray-200 shadow-sm mb-6">
        <TaskForm onAdd={handleAdd} />
      </div>
      <div className="flex items-center gap-2 mb-3">
        <span className="text-sm text-gray-600">Filter:</span>
        <button
          className={`px-2 py-1 rounded text-sm ${filter === 'all' ? 'bg-blue-600 text-white' : 'bg-gray-200'}`}
          onClick={() => setFilter('all')}
        >All</button>
        <button
          className={`px-2 py-1 rounded text-sm ${filter === 'active' ? 'bg-blue-600 text-white' : 'bg-gray-200'}`}
          onClick={() => setFilter('active')}
        >Active</button>
        <button
          className={`px-2 py-1 rounded text-sm ${filter === 'completed' ? 'bg-blue-600 text-white' : 'bg-gray-200'}`}
          onClick={() => setFilter('completed')}
        >Completed</button>
      </div>
      {loading && <div className="text-gray-600 mb-2">Loading...</div>}
      {error && <div className="text-red-600 mb-2">{error}</div>}
      <ul className="space-y-2">
        {filtered.map(task => (
          <TaskItem key={task.id} task={task} onToggle={handleToggle} onDelete={handleDelete} />
        ))}
        {!loading && filtered.length === 0 && (
          <div className="text-sm text-gray-500">No tasks to show.</div>
        )}
      </ul>
    </div>
  )
}

export default TaskList


