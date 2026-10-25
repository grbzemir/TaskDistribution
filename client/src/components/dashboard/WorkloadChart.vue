<script setup>
import { computed } from 'vue'
import { Bar } from 'vue-chartjs'
import { BarElement, CategoryScale, Chart as ChartJS, Legend, LinearScale, Tooltip } from 'chart.js'

ChartJS.register(BarElement, CategoryScale, LinearScale, Tooltip, Legend)

const props = defineProps({
  developers: { type: Array, required: true },
})

const TIERS = [
  { label: 'Easy (1–3)', levels: [1, 2, 3], color: '#22c55e' },
  { label: 'Medium (4–5)', levels: [4, 5], color: '#f59e0b' },
  { label: 'Hard (6–8)', levels: [6, 7, 8], color: '#ef4444' },
]

function workloadForLevels(developer, levels) {
  return levels.reduce((total, level) => total + level * developer.countsByDifficulty[level - 1], 0)
}

const chartData = computed(() => ({
  labels: props.developers.map((d) => (d.isActive ? d.fullName : `${d.fullName} (inactive)`)),
  datasets: TIERS.map((tier) => ({
    label: tier.label,
    data: props.developers.map((d) => workloadForLevels(d, tier.levels)),
    backgroundColor: tier.color,
    borderRadius: 4,
    maxBarThickness: 36,
  })),
}))

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { position: 'bottom', labels: { usePointStyle: true, boxWidth: 8 } },
    tooltip: {
      callbacks: {
        footer: (items) => `Total workload: ${items.reduce((sum, item) => sum + item.parsed.y, 0)}`,
      },
    },
  },
  scales: {
    x: { stacked: true, grid: { display: false } },
    y: { stacked: true, beginAtZero: true, ticks: { precision: 0 }, title: { display: true, text: 'Workload points' } },
  },
}
</script>

<template>
  <div class="chart-container">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>