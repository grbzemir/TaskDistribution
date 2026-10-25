import httpClient from './httpClient'

export const taskTypesApi = {
    getAll: () => httpClient.get('/task-types'),
    getById: (id) => httpClient.get(`/task-types/${id}`),
    create: (payload) => httpClient.post('/task-types', payload),
    update: (id, payload) => httpClient.put(`/task-types/${id}`, payload),
    remove: (id) => httpClient.delete(`/task-types/${id}`),
}