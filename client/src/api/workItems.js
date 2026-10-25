import httpClient from './httpClient'

export const workItemsApi = {
    getAll: (status) => httpClient.get('/work-items', { params: status ? { status } : {} }),
    getById: (id) => httpClient.get(`/work-items/${id}`),
    create: (payload) => httpClient.post('/work-items', payload),
    update: (id, payload) => httpClient.put(`/work-items/${id}`, payload),
    analyze: (id, taskTypeId) => httpClient.post(`/work-items/${id}/analyze`, { taskTypeId }),
    assign: (id) => httpClient.post(`/work-items/${id}/assign`),
    complete: (id) => httpClient.post(`/work-items/${id}/complete`),
    remove: (id) => httpClient.delete(`/work-items/${id}`),
}