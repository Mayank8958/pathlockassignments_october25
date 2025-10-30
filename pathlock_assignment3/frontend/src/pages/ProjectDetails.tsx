import { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import api from '../lib/api'

type Task = { id: number, title: string, dueDate?: string | null, isCompleted: boolean, createdAt: string }
type ScheduleTask = { taskId: number, title: string, estimatedHours: number }
type ScheduleDay = { date: string, tasks: ScheduleTask[], totalHours: number }
type ScheduleResponse = { projectId: number, startDate: string, schedule: ScheduleDay[], overflow?: ScheduleTask[] }

export default function ProjectDetails() {
  const { projectId } = useParams()
  const [tasks, setTasks] = useState<Task[]>([])
  const [title, setTitle] = useState('')
  const [dueDate, setDueDate] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // Scheduler UI state
  const [days, setDays] = useState(5)
  const [hoursPerDay, setHoursPerDay] = useState(4)
  const [startDateInput, setStartDateInput] = useState('')
  const [strategy, setStrategy] = useState<'earliest_due' | 'longest_task' | 'priority'>('earliest_due')
  const [scheduleLoading, setScheduleLoading] = useState(false)
  const [schedule, setSchedule] = useState<ScheduleResponse | null>(null)

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

  async function generateSchedule() {
    setScheduleLoading(true)
    setError(null)
    try {
      const payload: any = { days, hoursPerDay, strategy }
      if (startDateInput) payload.startDate = new Date(startDateInput)
      const { data } = await api.post<ScheduleResponse>(`/v1/projects/${projectId}/schedule`, payload)
      // Normalize date strings for rendering
      data.schedule = data.schedule.map(d => ({ ...d, date: new Date(d.date).toISOString() }))
      setSchedule(data)
    } catch (e: any) {
      setError(e.response?.data?.message || 'Failed to generate schedule')
    } finally {
      setScheduleLoading(false)
    }
  }

  function download(filename: string, content: string, type = 'application/json') {
    const blob = new Blob([content], { type })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = filename
    a.click()
    URL.revokeObjectURL(url)
  }

  function exportJson() {
    if (!schedule) return
    download(`schedule-project-${schedule.projectId}.json`, JSON.stringify(schedule, null, 2))
  }

  function exportCsv() {
    if (!schedule) return
    const lines: string[] = []
    lines.push('Date,Task ID,Title,Hours')
    schedule.schedule.forEach(d => {
      d.tasks.forEach(t => {
        const date = new Date(d.date).toISOString().substring(0,10)
        const title = '"' + (t.title?.replaceAll('"', '""') || '') + '"'
        lines.push([date, t.taskId, title, t.estimatedHours].join(','))
      })
    })
    if (schedule.overflow && schedule.overflow.length > 0) {
      lines.push('OVERFLOW,,,')
      schedule.overflow.forEach(t => {
        const title = '"' + (t.title?.replaceAll('"', '""') || '') + '"'
        lines.push(['', t.taskId, title, t.estimatedHours].join(','))
      })
    }
    download(`schedule-project-${schedule.projectId}.csv`, lines.join('\n'), 'text/csv')
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

      <h2 className="page-title" style={{marginTop:24}}>Scheduler</h2>
      <div className="card" style={{marginBottom:16}}>
        <div className="grid" style={{maxWidth:720, alignItems:'end'}}>
          <div>
            <label className="label">Days</label>
            <input className="input" type="number" min={1} max={365} value={days} onChange={e => setDays(parseInt(e.target.value || '1'))} />
          </div>
          <div>
            <label className="label">Hours / day</label>
            <input className="input" type="number" min={1} max={24} value={hoursPerDay} onChange={e => setHoursPerDay(parseInt(e.target.value || '1'))} />
          </div>
          <div>
            <label className="label">Start date</label>
            <input className="input" type="date" value={startDateInput} onChange={e => setStartDateInput(e.target.value)} />
          </div>
          <div>
            <label className="label">Strategy</label>
            <select className="input" value={strategy} onChange={e => setStrategy(e.target.value as any)}>
              <option value="earliest_due">Earliest due</option>
              <option value="longest_task">Longest task</option>
            </select>
          </div>
          <div>
            <button className="btn primary" onClick={generateSchedule} disabled={scheduleLoading}>
              {scheduleLoading ? 'Scheduling…' : 'Generate schedule'}
            </button>
          </div>
          {error && <div className="error" style={{gridColumn:'1 / -1'}}>{error}</div>}
        </div>
      </div>

      {schedule && (
        <div className="card">
          <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:8}}>
            <h3 className="page-title" style={{margin:0}}>Plan starting {new Date(schedule.startDate).toISOString().substring(0,10)}</h3>
            <div style={{display:'flex', gap:8}}>
              <button className="btn" onClick={exportJson}>Export JSON</button>
              <button className="btn" onClick={exportCsv}>Export CSV</button>
            </div>
          </div>
          <div className="grid" style={{gap:12}}>
            {schedule.schedule.map(day => (
              <div key={day.date} className="card" style={{padding:12}}>
                <strong>{new Date(day.date).toISOString().substring(0,10)}</strong>
                <ul className="list" style={{marginTop:8}}>
                  {day.tasks.map((t, i) => (
                    <li key={i} className="item" style={{display:'flex', justifyContent:'space-between'}}>
                      <span>{t.title} <span style={{opacity:.7}}>(#{t.taskId})</span></span>
                      <span>{t.estimatedHours}h</span>
                    </li>
                  ))}
                </ul>
                <div style={{textAlign:'right', opacity:.8}}>Total: {day.totalHours}h</div>
              </div>
            ))}
          </div>
          {schedule.overflow && schedule.overflow.length > 0 && (
            <div style={{marginTop:16}}>
              <h4>Overflow</h4>
              <ul className="list">
                {schedule.overflow.map((t, i) => (
                  <li key={i} className="item" style={{display:'flex', justifyContent:'space-between'}}>
                    <span>{t.title} <span style={{opacity:.7}}>(#{t.taskId})</span></span>
                    <span>{t.estimatedHours}h</span>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
    </div>
  )
}


