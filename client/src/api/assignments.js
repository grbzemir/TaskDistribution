import httpClient from './httpClient'

export const assignmentsApi = {
    getRules: () => httpClient.get('/assignments/rules'),
    getLogs: (count = 50) => httpClient.get('/assignments/logs', { params: { count } }),
}