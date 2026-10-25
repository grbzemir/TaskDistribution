import httpClient from './httpClient'

export const rolesApi = {
    getAll: () => httpClient.get('/roles'),
}