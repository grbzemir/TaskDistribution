const dateTimeFormatter = new Intl.DateTimeFormat('en-GB', {
    dateStyle: 'medium',
    timeStyle: 'short',
})

const relativeTimeFormatter = new Intl.RelativeTimeFormat('en', { numeric: 'auto' })

export function formatDateTime(value) {
    return value ? dateTimeFormatter.format(new Date(value)) : '—'
}

export function formatRelativeTime(value) {
    if (!value) {
        return '—'
    }

    const diffInSeconds = Math.round((new Date(value).getTime() - Date.now()) / 1000)
    const units = [
        ['day', 86400],
        ['hour', 3600],
        ['minute', 60],
    ]

    for (const [unit, seconds] of units) {
        if (Math.abs(diffInSeconds) >= seconds) {
            return relativeTimeFormatter.format(Math.round(diffInSeconds / seconds), unit)
        }
    }

    return 'just now'
}