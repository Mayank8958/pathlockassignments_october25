import { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import api from '../lib/api'

type Task = { id: number, title: string, dueDate?: string | null, isCompleted: boolean, createdAt: string }

export default function ProjectDetails() {
  const { projectId } = useParams()
  const [tasks, setTasks] = useState<Task[]>([])
  const [title, setTitle] = useState('')
  const [dueDate, setDueDate] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    try {
      const { data } = await api.get<Task[]>(`/projects/${projectId}/tasks`)
      setTasks(data)
    } catch (e: any) {
      setError(e.response?.data?.message || 'Failed to load tasks')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { load() }, [projectId])

  async function addTask() {
    if (!title) { setError('Task title is required'); return }
    const payload: any = { title }
    if (dueDate) payload.dueDate = new Date(dueDate)
    const { data } = await api.post(`/projects/${projectId}/tasks`, payload)
    setTasks(t => [data, ...t]); setTitle(''); setDueDate('')
  }

  async function toggleTask(id: number) {
    const { data } = await api.patch(`/projects/${projectId}/tasks/${id}/toggle`)
    setTasks(t => t.map(x => x.id === id ? { ...x, isCompleted: data.isCompleted } : x))
  }

  async function updateTask(id: number, next: Task) {
    const { data } = await api.put(`/projects/${projectId}/tasks/${id}`, {
      title: next.title,
      dueDate: next.dueDate ? new Date(next.dueDate) : null,
      isCompleted: next.isCompleted
    })
    setTasks(t => t.map(x => x.id === id ? data : x))
  }

  async function deleteTask(id: number) {
    await api.delete(`/projects/${projectId}/tasks/${id}`)
    setTasks(t => t.filter(x => x.id !== id))
  }

  return (
    <div>
      <p><Link to="/">← Back</Link></p>
      <h2 className="page-title">Project Tasks</h2>
      <div className="card" style={{marginBottom:16}}>
        <div className="grid" style={{maxWidth:560}}>
          <input className="input" placeholder="Task title" value={title} onChange={e => setTitle(e.target.value)} />
          <input className="input" type="date" value={dueDate} onChange={e => setDueDate(e.target.value)} />
          <button className="btn primary" onClick={addTask}>Add Task</button>
          {error && <div className="error">{error}</div>}
        </div>
      </div>
      {loading ? 'Loading…' : (
        <ul className="list">
          {tasks.map(t => (
            <li key={t.id} className="item" style={{display:'grid',gap:8}}>
              <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
                <input type="checkbox" checked={t.isCompleted} onChange={() => toggleTask(t.id)} />
                <input className="input" value={t.title} onChange={e => setTasks(xs => xs.map(x => x.id === t.id ? { ...x, title: e.target.value } : x))} />
                <input className="input" type="date" value={t.dueDate ? t.dueDate.substring(0,10) : ''} onChange={e => setTasks(xs => xs.map(x => x.id === t.id ? { ...x, dueDate: e.target.value ? new Date(e.target.value).toISOString() : null } : x))} />
              </div>
              <div style={{ display: 'flex', gap: 8 }}>
                <button className="btn" onClick={() => updateTask(t.id, t)}>Save</button>
                <button className="btn danger" onClick={() => deleteTask(t.id)}>Delete</button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}


