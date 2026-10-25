<script setup>
import AppModal from './AppModal.vue'

defineProps({
  show: { type: Boolean, default: false },
  title: { type: String, default: 'Are you sure?' },
  message: { type: String, default: '' },
  confirmLabel: { type: String, default: 'Confirm' },
  variant: { type: String, default: 'danger' },
  loading: { type: Boolean, default: false },
})

const emit = defineEmits(['confirm', 'cancel'])
</script>

<template>
  <AppModal :show="show" :title="title" @close="loading || emit('cancel')">
    <p class="mb-0 text-secondary">{{ message }}</p>
    <slot />

    <template #footer>
      <button type="button" class="btn btn-light" :disabled="loading" @click="emit('cancel')">
        Cancel
      </button>
      <button type="button" class="btn" :class="`btn-${variant}`" :disabled="loading" @click="emit('confirm')">
        <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
        {{ confirmLabel }}
      </button>
    </template>
  </AppModal>
</template>