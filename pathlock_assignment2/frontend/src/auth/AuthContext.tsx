import React, { createContext, useContext, useEffect, useMemo, useState } from 'react'
import api from '../lib/api'

type AuthContextType = {
  token: string | null
  isAuthenticated: boolean
  login: (token: string) => void
  logout: () => void
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem('token'))

  useEffect(() => {
    if (token) {
      localStorage.setItem('token', token)
      api.defaults.headers.common.Authorization = `Bearer ${token}`
    } else {
      localStorage.removeItem('token')
      delete api.defaults.headers.common.Authorization
    }
  }, [token])

  const value = useMemo(() => ({
    token,
    isAuthenticated: !!token,
    login: (t: string) => {
      localStorage.setItem('token', t)
      api.defaults.headers.common.Authorization = `Bearer ${t}`
      setToken(t)
    },
    logout: () => {
      localStorage.removeItem('token')
      delete (api.defaults.headers.common as any).Authorization
      setToken(null)
    }
  }), [token])

  // No request interceptor necessary; defaults are set above to avoid race conditions

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}


