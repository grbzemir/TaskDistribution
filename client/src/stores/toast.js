import { defineStore } from 'pinia'
import { ref } from 'vue'

let nextId = 1

export const useToastStore = defineStore('toast', () => {
    const toasts = ref([])

    function show(message, variant, timeout) {
        const id = nextId++
        toasts.value.push({ id, message, variant })

        if (timeout > 0) {
            setTimeout(() => dismiss(id), timeout)
        }
    }

    function dismiss(id) {
        toasts.value = toasts.value.filter((toast) => toast.id !== id)
    }

    return {
        toasts,
        dismiss,
        success: (message) => show(message, 'success', 4000),
        info: (message) => show(message, 'info', 4000),
        error: (message) => show(message, 'danger', 6000),
    }
})