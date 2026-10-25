import httpClient from './httpClient'

export const employeesApi = {
    getAll: () => httpClient.get('/employees'),
    getById: (id) => httpClient.get(`/employees/${id}`),
    create: (payload) => httpClient.post('/employees', payload),
    update: (id, payload) => httpClient.put(`/employees/${id}`, payload),
    remove: (id) => httpClient.delete(`/employees/${id}`),
}