<script setup>
import { computed } from 'vue'
import { DIFFICULTY_LEVELS } from '@/utils/difficulty'
import DifficultyBadge from '@/components/common/DifficultyBadge.vue'

const props = defineProps({
  developers: { type: Array, required: true },
})

const maxCount = computed(() =>
  Math.max(1, ...props.developers.flatMap((developer) => developer.countsByDifficulty)),
)

function cellStyle(count) {
  if (count === 0) {
    return {}
  }

  const intensity = count / maxCount.value

  return {
    backgroundColor: `rgba(15, 108, 189, ${0.12 + intensity * 0.68})`,
    color: intensity > 0.5 ? '#ffffff' : '#0c4a6e',
  }
}
</script>

<template>
  <div class="table-responsive">
    <table class="table heatmap-table mb-0">
      <thead>
        <tr>
          <th>Developer</th>
          <th v-for="level in DIFFICULTY_LEVELS" :key="level" class="text-center">
            <DifficultyBadge :level="level" />
          </th>
          <th class="text-end">Total</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="developer in developers" :key="developer.employeeId" :class="{ 'row-inactive': !developer.isActive }">
          <td class="text-nowrap small fw-medium">{{ developer.fullName }}</td>
          <td v-for="(count, index) in developer.countsByDifficulty" :key="index" class="p-1">
            <div class="heat-cell" :style="cellStyle(count)">{{ count || '·' }}</div>
          </td>
          <td class="text-end small fw-semibold">{{ developer.taskCount }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>