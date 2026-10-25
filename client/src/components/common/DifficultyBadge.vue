<script setup>
import { computed } from 'vue'

const props = defineProps({
  level: { type: Number, default: null },
})

const tier = computed(() => {
  if (props.level == null) return 'none'
  if (props.level <= 3) return 'low'
  if (props.level <= 5) return 'medium'
  return 'high'
})
</script>

<template>
  <span
    class="difficulty-badge"
    :class="`difficulty-${tier}`"
    :title="level ? `Difficulty ${level} of 8` : 'Not analyzed yet'"
  >
    {{ level ? `Lv ${level}` : '—' }}
  </span>
</template>