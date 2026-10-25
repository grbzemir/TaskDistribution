<script setup>
import { computed } from 'vue'
import { DIFFICULTY_MAX, getDifficultyTier } from '@/utils/difficulty'

const props = defineProps({
  level: { type: Number, default: null },
})

const tier = computed(() => getDifficultyTier(props.level))
</script>

<template>
  <span
    class="difficulty-badge"
    :class="`difficulty-${tier}`"
    :title="level ? `Difficulty ${level} of ${DIFFICULTY_MAX}` : 'Not analyzed yet'"
  >
    {{ level ? `Lv ${level}` : '—' }}
  </span>
</template>