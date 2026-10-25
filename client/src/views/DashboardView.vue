<script setup>
import { computed, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { assignmentsApi, dashboardApi } from '@/api'
import { formatRelativeTime, getInitials } from '@/utils/formatters'
import PageHeader from '@/components/common/PageHeader.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import DifficultyBadge from '@/components/common/DifficultyBadge.vue'
import WorkloadChart from '@/components/dashboard/WorkloadChart.vue'
import DistributionHeatmap from '@/components/dashboard/DistributionHeatmap.vue'

const summary = ref(null)
const recentLogs = ref([])
const isLoading = ref(true)
const loadError = ref('')

const statusCards = computed(() => [
  { label: 'Awaiting analysis', value: summary.value.newCount, icon: 'bi-inbox', tone: 'neutral' },
  { label: 'Ready to assign', value: summary.value.analyzedCount, icon: 'bi-search', tone: 'info' },
  { label: 'In progress', value: summary.value.assignedCount, icon: 'bi-person-workspace', tone: 'primary' },
  { label: 'Completed', value: summary.value.completedCount, icon: 'bi-check2-circle', tone: 'success' },
])

const fairnessPercent = computed(() => Math.round(summary.value.fairnessIndex * 100))

const fairnessLevel = computed(() => {
  if (fairnessPercent.value >= 95) return { label: 'Excellent', tone: 'success' }
  if (fairnessPercent.value >= 85) return { label: 'Good', tone: 'warning' }
  return { label: 'Uneven', tone: 'danger' }
})

const totalAssignments = computed(() =>
  summary.value.developers.reduce((total, developer) => total + developer.taskCount, 0),
)

const hasEarlyData = computed(() => totalAssignments.value < summary.value.activeDeveloperCount)

async function loadData() {
  isLoading.value = true
  loadError.value = ''

  try {
    const [summaryData, logs] = await Promise.all([dashboardApi.getSummary(), assignmentsApi.getLogs(5)])
    summary.value = summaryData
    recentLogs.value = logs
  } catch (error) {
    loadError.value = error.message
  } finally {
    isLoading.value = false
  }
}

onMounted(loadData)
</script>

<template>
  <PageHeader title="Dashboard" subtitle="How fairly work is distributed across the development team.">
    <template #actions>
      <button type="button" class="btn btn-light border" :disabled="isLoading" @click="loadData">
        <i class="bi bi-arrow-clockwise me-1"></i> Refresh
      </button>
    </template>
  </PageHeader>

  <LoadingState v-if="isLoading && !summary" label="Loading dashboard…" />

  <div v-else-if="loadError" class="surface">
    <EmptyState icon="bi-exclamation-triangle" title="Couldn't load the dashboard" :message="loadError">
      <button type="button" class="btn btn-outline-primary btn-sm" @click="loadData">Try again</button>
    </EmptyState>
  </div>

  <template v-else-if="summary">
    <div class="metric-grid">
      <div v-for="card in statusCards" :key="card.label" class="metric-card">
        <span class="metric-icon" :class="`metric-icon-${card.tone}`"><i class="bi" :class="card.icon"></i></span>
        <div>
          <div class="metric-label">{{ card.label }}</div>
          <div class="metric-value">{{ card.value }}</div>
        </div>
      </div>
    </div>

    <div class="dashboard-grid">
      <div class="surface">
        <div class="surface-header">
          <h2 class="surface-title">Workload by developer</h2>
          <span class="small text-secondary">{{ summary.activeDeveloperCount }} active developers</span>
        </div>
        <div class="surface-body">
          <WorkloadChart v-if="totalAssignments > 0" :developers="summary.developers" />
          <EmptyState
            v-else
            icon="bi-bar-chart"
            title="No assignments yet"
            message="Assign work items to see how the workload is distributed."
          />
        </div>
      </div>

      <div class="d-flex flex-column gap-3">
        <div class="surface fairness-card">
          <div class="d-flex align-items-start justify-content-between mb-2">
            <div>
              <div class="metric-label">Workload fairness</div>
              <div class="fairness-value">{{ fairnessPercent }}%</div>
            </div>
            <span class="fairness-pill" :class="`fairness-${fairnessLevel.tone}`">{{ fairnessLevel.label }}</span>
          </div>

          <div class="fairness-track">
            <div class="fairness-fill" :class="`fairness-fill-${fairnessLevel.tone}`" :style="{ width: `${fairnessPercent}%` }"></div>
          </div>

          <p class="small text-secondary mt-3 mb-2">
            How evenly the workload is shared among active developers. 100% means everyone carries the same load.
          </p>

          <div class="d-flex justify-content-between small border-top pt-2">
            <span class="text-secondary">Max spread per difficulty level</span>
            <span class="fw-semibold">{{ summary.maxSpreadPerDifficulty }} task(s)</span>
          </div>

          <p v-if="hasEarlyData" class="small text-secondary mt-2 mb-0">
            <i class="bi bi-info-circle me-1"></i>
            Some developers haven't received a task yet, so the score is still settling.
          </p>
        </div>

        <div class="surface">
          <div class="surface-header">
            <h2 class="surface-title">Recent assignments</h2>
            <RouterLink to="/assignments" class="small">View all</RouterLink>
          </div>

          <ul v-if="recentLogs.length" class="recent-list">
            <li v-for="log in recentLogs" :key="log.id">
              <span class="avatar avatar-sm">{{ getInitials(log.employeeName) }}</span>
              <div class="recent-text">
                <div class="small fw-medium text-truncate">{{ log.workItemTitle }}</div>
                <div class="small text-secondary">{{ log.employeeName }} · {{ formatRelativeTime(log.assignedAt) }}</div>
              </div>
              <DifficultyBadge :level="log.difficulty" />
            </li>
          </ul>
          <p v-else class="small text-secondary p-3 mb-0">No assignments yet.</p>
        </div>
      </div>
    </div>

    <div class="surface mt-3">
      <div class="surface-header">
        <h2 class="surface-title">Distribution by difficulty level</h2>
        <span class="small text-secondary">Similar colors within a column mean a fair spread</span>
      </div>
      <div class="surface-body">
        <DistributionHeatmap :developers="summary.developers" />
      </div>
    </div>
  </template>
</template>