<script setup>
import { DIFFICULTY_LEVELS, getDifficultyTier } from '@/utils/difficulty'

const model = defineModel({ type: Number, default: null })

const props = defineProps({
  takenLevels: { type: Array, default: () => [] },
  invalid: { type: Boolean, default: false },
})

function isTaken(level) {
  return props.takenLevels.includes(level)
}

function select(level) {
  if (!isTaken(level)) {
    model.value = level
  }
}
</script>

<template>
  <div
    class="difficulty-picker"
    :class="{ 'is-invalid': invalid }"
    role="radiogroup"
    aria-label="Difficulty level"
  >
    <button
      v-for="level in DIFFICULTY_LEVELS"
      :key="level"
      type="button"
      class="difficulty-option"
      :class="[`difficulty-option-${getDifficultyTier(level)}`, { selected: model === level }]"
      role="radio"
      :aria-checked="model === level"
      :disabled="isTaken(level)"
      :title="isTaken(level) ? `Level ${level} is used by another task type` : `Level ${level}`"
      @click="select(level)"
    >
      {{ level }}
    </button>
  </div>
  <div class="difficulty-picker-legend">
    <span>Easier</span>
    <span>Harder</span>
  </div>
</template>v