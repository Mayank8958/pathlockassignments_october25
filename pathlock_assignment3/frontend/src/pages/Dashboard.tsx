import { useEffect, useState } from 'react'
import api from '../lib/api'
import { Link } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

type Project = { id: number, title: string, description?: string, createdAt: string }

export default function Dashboard() {
  const { isAuthenticated } = useAuth()
  const [projects, setProjects] = useState<Project[]>([])
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    try {
      const { data } = await api.get<Project[]>('/projects')
      setProjects(data)
    } catch (e: any) {
      setError(e.response?.data?.message || 'Failed to load projects')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (isAuthenticated) load()
  }, [isAuthenticated])

  async function createProject() {
    if (title.length < 3) { setError('Title must be at least 3 characters'); return }
    try {
      const { data } = await api.post<Project>('/projects', { title, description: description || undefined })
      setProjects(p => [data, ...p])
      setTitle(''); setDescription('')
    } catch (e: any) {
      setError(e.response?.data?.message || 'Failed to create project')
    }
  }

  async function removeProject(id: number) {
    if (!confirm('Delete this project?')) return
    await api.delete(`/projects/${id}`)
    setProjects(p => p.filter(x => x.id !== id))
  }

  return (
    <div>
      <h2 className="page-title">Your Projects</h2>
      <div className="card" style={{marginBottom:16}}>
        <div className="grid" style={{maxWidth:560}}>
          <input className="input" placeholder="Project title" value={title} onChange={e => setTitle(e.target.value)} />
          <input className="input" placeholder="Description (optional)" value={description} onChange={e => setDescription(e.target.value)} />
          <button className="btn primary" onClick={createProject}>Create Project</button>
          {error && <div className="error">{error}</div>}
        </div>
      </div>
      {loading ? 'Loading…' : (
        <ul className="list">
          {projects.map(p => (
            <li key={p.id} className="item">
              <div>
                <Link to={`/projects/${p.id}`} className="title">{p.title}</Link>
                {p.description && <div className="muted">{p.description}</div>}
              </div>
              <button className="btn danger" onClick={() => removeProject(p.id)}>Delete</button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}


