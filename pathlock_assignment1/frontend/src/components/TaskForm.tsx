import React, { useState } from 'react'

type Props = {
  onAdd: (title: string, description?: string) => Promise<void>
}

const TaskForm: React.FC<Props> = ({ onAdd }) => {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)
    if (!title.trim()) {
      setError('Title is required')
      return
    }
    setLoading(true)
    try {
      await onAdd(title.trim(), description.trim() || undefined)
      setTitle('')
      setDescription('')
    } catch (err: any) {
      setError(err?.message ?? 'Failed to add task')
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-3">
      {error && <div className="text-red-600 text-sm">{error}</div>}
      <input
        className="w-full border rounded px-3 py-2"
        placeholder="Task title"
        maxLength={200}
        value={title}
        onChange={e => setTitle(e.target.value)}
      />
      <input
        className="w-full border rounded px-3 py-2"
        placeholder="Description (optional)"
        value={description}
        onChange={e => setDescription(e.target.value)}
      />
      <button
        disabled={loading}
        className="bg-blue-600 text-white px-4 py-2 rounded disabled:opacity-60"
      >
        {loading ? 'Adding...' : 'Add Task'}
      </button>
    </form>
  )
}

export default TaskForm


