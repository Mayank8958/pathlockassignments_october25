import { FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import api from '../lib/api'
import { useAuth } from '../auth/AuthContext'

export default function Login() {
  const { login } = useAuth()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    if (!username || !password) { setError('Username and password are required'); return }
    try {
      setLoading(true)
      const { data } = await api.post('/auth/login', { username, password })
      login(data.token)
      navigate('/')
    } catch (err: any) {
      setError(err.response?.data?.message || 'Login failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div>
      <div className="card" style={{maxWidth:420,margin:'0 auto'}}>
        <h2 className="page-title">Login</h2>
        <form onSubmit={onSubmit} className="grid">
          <div className="stack">
            <span className="label">Username</span>
            <input className="input" placeholder="Username" value={username} onChange={e => setUsername(e.target.value)} />
          </div>
          <div className="stack">
            <span className="label">Password</span>
            <input className="input" placeholder="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} />
          </div>
          {error && <div className="error">{error}</div>}
          <button className="btn primary" disabled={loading}>{loading ? 'Logging in…' : 'Login'}</button>
        </form>
        <p className="muted" style={{marginTop:8}}>New here? <Link to="/register">Create an account</Link></p>
      </div>
    </div>
  )
}


