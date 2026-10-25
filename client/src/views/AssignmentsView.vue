<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { assignmentsApi } from '@/api'
import { getRuleLabel } from '@/utils/assignmentRules'
import { formatDateTime, formatRelativeTime, getInitials } from '@/utils/formatters'
import PageHeader from '@/components/common/PageHeader.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import DifficultyBadge from '@/components/common/DifficultyBadge.vue'

const FAIRNESS_CRITERIA = [
  'Fewest tasks at the same difficulty level',
  'Lowest total workload (sum of difficulties)',
  'Longest time since the last assignment',
  'Lowest employee id, so results are deterministic',
]

const rules = ref([])
const logs = ref([])
const isLoading = ref(true)
const loadError = ref('')

const search = ref('')
const onlyRelaxed = ref(false)
const logLimit = ref(100)
const expandedLogId = ref(null)

const enabledRuleCount = computed(() => rules.value.filter((rule) => rule.isEnabled).length)
const relaxedCount = computed(() => logs.value.filter((log) => log.isRuleRelaxed).length)

const filteredLogs = computed(() => {
  const term = search.value.trim().toLocaleLowerCase('tr')

  return logs.value.filter((log) => {
    const matchesRelaxed = !onlyRelaxed.value || log.isRuleRelaxed
    const matchesSearch =
      !term ||
      log.employeeName.toLocaleLowerCase('tr').includes(term) ||
      log.workItemTitle.toLocaleLowerCase('tr').includes(term)

    return matchesRelaxed && matchesSearch
  })
})

async function loadData() {
  isLoading.value = true
  loadError.value = ''

  try {
    const [ruleList, logList] = await Promise.all([
      assignmentsApi.getRules(),
      assignmentsApi.getLogs(logLimit.value),
    ])

    rules.value = ruleList
    logs.value = logList
  } catch (error) {
    loadError.value = error.message
  } finally {
    isLoading.value = false
  }
}

function toggleLog(logId) {
  expandedLogId.value = expandedLogId.value === logId ? null : logId
}

watch(logLimit, loadData)

onMounted(loadData)
</script>

<template>
  <PageHeader
    title="Assignments"
    :subtitle="`${enabledRuleCount} of ${rules.length} rules enabled · ${logs.length} recent assignments · ${relaxedCount} with relaxed rules`"
  >
    <template #actions>
      <button type="button" class="btn btn-light border" :disabled="isLoading" @click="loadData">
        <i class="bi bi-arrow-clockwise me-1"></i> Refresh
      </button>
    </template>
  </PageHeader>

  <LoadingState v-if="isLoading && rules.length === 0" label="Loading assignments…" />

  <div v-else-if="loadError" class="surface">
    <EmptyState icon="bi-exclamation-triangle" title="Couldn't load assignments" :message="loadError">
      <button type="button" class="btn btn-outline-primary btn-sm" @click="loadData">Try again</button>
    </EmptyState>
  </div>

  <template v-else>
    <h2 class="section-title">Distribution rules</h2>

    <div class="rule-cards">
      <div v-for="rule in rules" :key="rule.key" class="rule-card">
        <div class="d-flex align-items-start justify-content-between gap-2 mb-2">
          <span class="rule-card-title">{{ rule.name }}</span>
          <span class="rule-state" :class="rule.isEnabled ? 'rule-state-on' : 'rule-state-off'">
            {{ rule.isEnabled ? 'Enabled' : 'Disabled' }}
          </span>
        </div>
        <p class="small text-secondary mb-2">{{ rule.description }}</p>
        <code class="small">{{ rule.key }}</code>
      </div>

      <div class="rule-card">
        <div class="rule-card-title mb-2">Fairness ordering</div>
        <p class="small text-secondary mb-2">Among developers who pass the enabled rules, the first one by:</p>
        <ol class="fairness-list">
          <li v-for="criterion in FAIRNESS_CRITERIA" :key="criterion">{{ criterion }}</li>
        </ol>
      </div>
    </div>

    <p class="small text-secondary mb-4">
      Rules are enabled in <code>appsettings.json</code> under <code>Assignment:EnabledRules</code> and take
      effect without restarting the API. When no developer satisfies every enabled rule, the rules are relaxed
      and the fairest developer is selected, so assignments never get stuck.
      Her yeni görev, aktif kuralları geçen en adil developer’a gidiyor Kimse geçemezse o görev için kurallar esnetmek lazım
      Buraya ekstra kontrol yönetici neden bunu bilsin ki.
    </p>

    <h2 class="section-title">Assignment history</h2>

    <div class="surface">
      <div class="toolbar">
        <div class="search-input">
          <i class="bi bi-search"></i>
          <input v-model="search" type="search" class="form-control" placeholder="Search developer or work item" />
        </div>

        <div class="form-check form-switch mb-0">
          <input id="only-relaxed" v-model="onlyRelaxed" class="form-check-input" type="checkbox" role="switch" />
          <label class="form-check-label small" for="only-relaxed">Only relaxed</label>
        </div>

        <select v-model.number="logLimit" class="form-select ms-auto" aria-label="Number of records">
          <option :value="50">Last 50</option>
          <option :value="100">Last 100</option>
          <option :value="200">Last 200</option>
        </select>
      </div>

      <LoadingState v-if="isLoading" label="Refreshing…" />

      <EmptyState
        v-else-if="logs.length === 0"
        icon="bi-diagram-3"
        title="No assignments yet"
        message="Assign an analyzed work item to see the history here."
      />

      <EmptyState
        v-else-if="filteredLogs.length === 0"
        icon="bi-funnel"
        title="No assignments match"
        message="Try another search or turn off the relaxed filter."
      />

      <div v-else class="table-responsive">
        <table class="table app-table">
          <thead>
            <tr>
              <th>When</th>
              <th>Work item</th>
              <th>Developer</th>
              <th>Level</th>
              <th>Outcome</th>
              <th class="text-end" style="width: 48px"><span class="visually-hidden">Details</span></th>
            </tr>
          </thead>
          <tbody>
            <template v-for="log in filteredLogs" :key="log.id">
              <tr
                class="log-row"
                :class="{ expanded: expandedLogId === log.id }"
                :aria-expanded="expandedLogId === log.id"
                @click="toggleLog(log.id)"
              >
                <td class="small text-nowrap" :title="formatDateTime(log.assignedAt)">
                  {{ formatRelativeTime(log.assignedAt) }}
                </td>
                <td>
                  <div class="fw-medium">{{ log.workItemTitle }}</div>
                  <div class="small text-secondary">#{{ log.workItemId }}</div>
                </td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <span class="avatar avatar-sm">{{ getInitials(log.employeeName) }}</span>
                    <span class="small text-nowrap">{{ log.employeeName }}</span>
                  </div>
                </td>
                <td><DifficultyBadge :level="log.difficulty" /></td>
                <td>
                  <span v-if="log.isRuleRelaxed" class="outcome-badge outcome-relaxed">
                    <i class="bi bi-exclamation-triangle"></i> Rules relaxed
                  </span>
                  <span v-else class="outcome-badge outcome-ok">
                    <i class="bi bi-shield-check"></i> Rules satisfied
                  </span>
                </td>
                <td class="text-end">
                  <i class="bi text-secondary" :class="expandedLogId === log.id ? 'bi-chevron-up' : 'bi-chevron-down'"></i>
                </td>
              </tr>

              <tr v-if="expandedLogId === log.id" class="log-detail-row">
                <td colspan="6">
                  <div class="reason-box">
                    <div class="reason-label"><i class="bi bi-chat-square-text"></i> Why this developer?</div>
                    <p class="mb-2">{{ log.reason }}</p>
                    <div class="d-flex flex-wrap gap-2">
                      <span v-for="rule in log.appliedRules" :key="rule" class="rule-chip">
                        <i class="bi bi-shield-check"></i> {{ getRuleLabel(rule) }}
                      </span>
                      <span v-if="log.appliedRules.length === 0" class="small text-secondary">
                        No rules were enabled for this assignment.
                      </span>
                    </div>
                    <div class="small text-secondary mt-2">{{ formatDateTime(log.assignedAt) }}</div>
                  </div>
                </td>
              </tr>
            </template>
          </tbody>
        </table>
      </div>
    </div>
  </template>
</template>