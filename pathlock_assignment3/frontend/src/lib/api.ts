import axios from 'axios'

const api = axios.create({
  baseURL: '/api'
})

// Set token immediately on startup to avoid first-call race condition
const startupToken = localStorage.getItem('token')
if (startupToken) {
  api.defaults.headers.common.Authorization = `Bearer ${startupToken}`
}

api.interceptors.response.use(
  res => res,
  err => {
    const status = err.response?.status
    const url: string | undefined = err.config?.url
    const isAuthCall = !!url && (url.includes('/auth/login') || url.includes('/auth/register'))
    if (status === 401 && !isAuthCall) {
      localStorage.removeItem('token')
      if (location.pathname !== '/login') {
        location.href = '/login'
      }
    }
    return Promise.reject(err)
  }
)

export default api


