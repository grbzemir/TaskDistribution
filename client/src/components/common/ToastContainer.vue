<script setup>
import { storeToRefs } from 'pinia'
import { useToastStore } from '@/stores/toast'

const toastStore = useToastStore()
const { toasts } = storeToRefs(toastStore)

const icons = {
  success: 'bi-check-circle-fill',
  danger: 'bi-exclamation-octagon-fill',
  info: 'bi-info-circle-fill',
}
</script>

<template>
  <div class="toast-stack" aria-live="polite">
    <TransitionGroup name="toast">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        class="app-toast"
        :class="`app-toast-${toast.variant}`"
        role="status"
      >
        <i class="bi" :class="icons[toast.variant]"></i>
        <span class="flex-grow-1">{{ toast.message }}</span>
        <button
          type="button"
          class="btn-close"
          style="font-size: 0.65rem"
          aria-label="Dismiss"
          @click="toastStore.dismiss(toast.id)"
        ></button>
      </div>
    </TransitionGroup>
  </div>
</template>