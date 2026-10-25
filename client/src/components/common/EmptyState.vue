<script setup>
defineProps({
  icon: { type: String, default: 'bi-inbox' },
  title: { type: String, required: true },
  message: { type: String, default: '' },
})
</script>

<template>
  <div class="empty-state">
    <div class="empty-state-icon"><i class="bi" :class="icon"></i></div>
    <div class="empty-state-title">{{ title }}</div>
    <p v-if="message" class="mb-0">{{ message }}</p>
    <div v-if="$slots.default" class="mt-3">
      <slot />
    </div>
  </div>
</template>