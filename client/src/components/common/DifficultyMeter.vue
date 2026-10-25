<script setup>
import { DIFFICULTY_LEVELS, DIFFICULTY_MAX, getDifficultyTier } from '@/utils/difficulty'

defineProps({
  level: { type: Number, required: true },
})
</script>

<template>
  <div
    class="difficulty-meter"
    :class="`difficulty-meter-${getDifficultyTier(level)}`"
    role="img"
    :aria-label="`Difficulty ${level} of ${DIFFICULTY_MAX}`"
  >
    <span
      v-for="step in DIFFICULTY_LEVELS"
      :key="step"
      class="difficulty-meter-step"
      :class="{ filled: step <= level }"
    ></span>
  </div>
</template>