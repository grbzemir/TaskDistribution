<script setup>
const props = defineProps({
  status: { type: String, required: true },
})

const icons = {
  New: 'bi-circle',
  Analyzed: 'bi-search',
  Assigned: 'bi-person-check',
  Completed: 'bi-check2-circle',
}
</script>

<template>
  <span class="status-badge" :class="`status-${props.status.toLowerCase()}`">
    <i class="bi" :class="icons[props.status] ?? 'bi-circle'"></i>
    {{ props.status }}
  </span>
</template>