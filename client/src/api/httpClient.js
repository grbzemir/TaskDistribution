import axios from 'axios'

export class ApiError extends Error {
    constructor({ status, title, detail, errors }) {
        super(detail || title || 'Something went wrong.')
        this.name = 'ApiError'
        this.status = status
        this.title = title
        this.fieldErrors = errors ?? {}
    }
}

const httpClient = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL ?? '/api',
    headers: { 'Content-Type': 'application/json' },
    timeout: 15000,
})

httpClient.interceptors.response.use(
    (response) => response.data,
    (error) => {
        if (error.response) {
            const { status, data } = error.response

            return Promise.reject(
                new ApiError({         status,
                    title: data?.title,
                    detail: data?.detail,
                    errors: data?.errors,
           
                }),
            )
        }

        const isTimeout = error.code === 'ECONNABORTED'

        return Promise.reject(
            new ApiError({
                status: 0,
                title: isTimeout ? 'Request timed out' : 'Network error',
                detail: isTimeout
                    ? 'The server took too long to respond. Please try again.'
                    : 'The server could not be reached. Make sure the API is running.',
            }),
        )
    },
)

export default httpClient