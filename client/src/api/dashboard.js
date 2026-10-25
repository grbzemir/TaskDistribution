import httpClient from './httpClient'

export const dashboardApi = {
    getSummary: () => httpClient.get('/dashboard'),
}