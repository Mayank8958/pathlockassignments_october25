import axios from 'axios'

export interface TaskDto {
  id: number
  title: string
  description?: string | null
  isCompleted: boolean
  createdAt: string
}

export interface CreateTaskDto {
  title: string
  description?: string
}

export interface UpdateTaskDto {
  title: string
  description?: string
  isCompleted: boolean
}

const api = axios.create({
  baseURL: 'http://localhost:5000',
})

export const getTasks = async (): Promise<TaskDto[]> => {
  const res = await api.get<TaskDto[]>('/api/tasks')
  return res.data
}

export const getTask = async (id: number): Promise<TaskDto> => {
  const res = await api.get<TaskDto>(`/api/tasks/${id}`)
  return res.data
}

export const createTask = async (payload: CreateTaskDto): Promise<TaskDto> => {
  const res = await api.post<TaskDto>('/api/tasks', payload)
  return res.data
}

export const updateTask = async (id: number, payload: UpdateTaskDto): Promise<void> => {
  await api.put(`/api/tasks/${id}`, payload)
}

export const deleteTask = async (id: number): Promise<void> => {
  await api.delete(`/api/tasks/${id}`)
}


