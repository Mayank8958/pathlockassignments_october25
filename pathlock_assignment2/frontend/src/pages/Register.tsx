import { FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import api from '../lib/api'
import { useAuth } from '../auth/AuthContext'

export default function Register() {
  const { login } = useAuth()
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    if (username.length < 3) { setError('Username must be at least 3 characters'); return }
    if (password.length < 6) { setError('Password must be at least 6 characters'); return }
    try {
      setLoading(true)
      const { data } = await api.post('/auth/register', { username, password, email: email || undefined })
      login(data.token)
      navigate('/')
    } catch (err: any) {
      setError(err.response?.data?.message || 'Registration failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div>
      <div className="card" style={{maxWidth:420,margin:'0 auto'}}>
        <h2 className="page-title">Create account</h2>
        <form onSubmit={onSubmit} className="grid">
          <div className="stack">
            <span className="label">Username</span>
            <input className="input" placeholder="Username" value={username} onChange={e => setUsername(e.target.value)} />
          </div>
          <div className="stack">
            <span className="label">Email (optional)</span>
            <input className="input" placeholder="you@example.com" value={email} onChange={e => setEmail(e.target.value)} />
          </div>
          <div className="stack">
            <span className="label">Password</span>
            <input className="input" placeholder="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} />
          </div>
          {error && <div className="error">{error}</div>}
          <button className="btn primary" disabled={loading}>{loading ? 'Creating…' : 'Create account'}</button>
        </form>
        <p className="muted" style={{marginTop:8}}>Already have an account? <Link to="/login">Login</Link></p>
      </div>
    </div>
  )
}


