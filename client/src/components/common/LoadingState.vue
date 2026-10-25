<script setup>
defineProps({
  label: { type: String, default: 'Loading…' },
})
</script>

<template>
  <div class="loading-state" role="status">
    <div class="spinner-border spinner-border-sm text-primary"></div>
    <span>{{ label }}</span>
  </div>
</template>