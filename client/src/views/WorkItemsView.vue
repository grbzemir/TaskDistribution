<script setup>
import { computed, onMounted, ref } from 'vue'
import { taskTypesApi, workItemsApi } from '@/api'
import { useToastStore } from '@/stores/toast'
import { formatDateTime, formatRelativeTime, getInitials } from '@/utils/formatters'
import PageHeader from '@/components/common/PageHeader.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ConfirmDialog from '@/components/common/ConfirmDialog.vue'
import DifficultyBadge from '@/components/common/DifficultyBadge.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import WorkItemFormModal from '@/components/workItems/WorkItemFormModal.vue'
import AnalyzeWorkItemModal from '@/components/workItems/AnalyzeWorkItemModal.vue'
import AssignmentResultModal from '@/components/workItems/AssignmentResultModal.vue'

const STATUS_TABS = ['All', 'New', 'Analyzed', 'Assigned', 'Completed']

const toast = useToastStore()

const workItems = ref([])
const taskTypes = ref([])
const isLoading = ref(true)
const loadError = ref('')

const search = ref('')
const statusFilter = ref('All')
const busyItemId = ref(null)

const isFormOpen = ref(false)
const editingItem = ref(null)
const analyzeTarget = ref(null)
const assignmentResult = ref(null)

const deleteTarget = ref(null)
const isDeleting = ref(false)

const statusCounts = computed(() => {
  const counts = { All: workItems.value.length, New: 0, Analyzed: 0, Assigned: 0, Completed: 0 }

  for (const item of workItems.value) {
    counts[item.status]++
  }

  return counts
})

const filteredItems = computed(() => {
  const term = search.value.trim().toLocaleLowerCase('tr')

  return workItems.value.filter((item) => {
    const matchesStatus = statusFilter.value === 'All' || item.status === statusFilter.value
    const matchesSearch = !term || item.title.toLocaleLowerCase('tr').includes(term)
    return matchesStatus && matchesSearch
  })
})

function lastActivity(item) {
  return item.completedAt ?? item.assignedAt ?? item.analyzedAt ?? item.createdAt
}

async function loadData() {
  isLoading.value = true
  loadError.value = ''

  try {
    const [itemList, typeList] = await Promise.all([workItemsApi.getAll(), taskTypesApi.getAll()])
    workItems.value = itemList
    taskTypes.value = typeList
  } catch (error) {
    loadError.value = error.message
  } finally {
    isLoading.value = false
  }
}

function replaceItem(updated) {
  workItems.value = workItems.value.map((item) => (item.id === updated.id ? updated : item))
}

function openCreate() {
  editingItem.value = null
  isFormOpen.value = true
}

function openEdit(item) {
  editingItem.value = item
  isFormOpen.value = true
}

function handleSaved(saved) {
  if (editingItem.value) {
    replaceItem(saved)
    toast.success('Work item was updated.')
  } else {
    workItems.value = [saved, ...workItems.value]
    toast.success('Work item was created. Analyze it to set its difficulty.')
  }

  isFormOpen.value = false
}

function handleAnalyzed(item) {
  replaceItem(item)
  analyzeTarget.value = null
  toast.success(`Analyzed as ${item.taskTypeName} (level ${item.difficulty}).`)
}

function handleAssigned(result) {
  replaceItem(result.workItem)
  analyzeTarget.value = null
  assignmentResult.value = result
}

async function assign(item) {
  busyItemId.value = item.id

  try {
    handleAssigned(await workItemsApi.assign(item.id))
  } catch (error) {
    toast.error(error.message)
  } finally {
    busyItemId.value = null
  }
}

async function complete(item) {
  busyItemId.value = item.id

  try {
    const updated = await workItemsApi.complete(item.id)
    replaceItem(updated)
    toast.success(`"${updated.title}" was completed.`)
  } catch (error) {
    toast.error(error.message)
  } finally {
    busyItemId.value = null
  }
}

async function confirmDelete() {
  const target = deleteTarget.value
  isDeleting.value = true

  try {
    await workItemsApi.remove(target.id)
    workItems.value = workItems.value.filter((item) => item.id !== target.id)
    toast.success('Work item was deleted.')
  } catch (error) {
    toast.error(error.message)
  } finally {
    deleteTarget.value = null
    isDeleting.value = false
  }
}

onMounted(loadData)
</script>

<template>
  <PageHeader title="Work items" subtitle="Create tasks, let analysts set their difficulty and assign them fairly.">
    <template #actions>
      <button type="button" class="btn btn-primary" @click="openCreate">
        <i class="bi bi-plus-lg me-1"></i> New work item
      </button>
    </template>
  </PageHeader>

  <div class="surface">
    <div class="toolbar">
      <div class="status-tabs" role="tablist" aria-label="Filter by status">
        <button
          v-for="tab in STATUS_TABS"
          :key="tab"
          type="button"
          class="status-tab"
          :class="{ active: statusFilter === tab }"
          role="tab"
          :aria-selected="statusFilter === tab"
          @click="statusFilter = tab"
        >
          {{ tab }}
          <span class="status-tab-count">{{ statusCounts[tab] }}</span>
        </button>
      </div>

      <div class="search-input ms-auto">
        <i class="bi bi-search"></i>
        <input v-model="search" type="search" class="form-control" placeholder="Search by title" />
      </div>
    </div>

    <LoadingState v-if="isLoading" label="Loading work items…" />

    <EmptyState v-else-if="loadError" icon="bi-exclamation-triangle" title="Couldn't load work items" :message="loadError">
      <button type="button" class="btn btn-outline-primary btn-sm" @click="loadData">Try again</button>
    </EmptyState>

    <EmptyState
      v-else-if="workItems.length === 0"
      icon="bi-kanban"
      title="Create your first work item"
      message="Work items are analyzed by analysts and then assigned to developers automatically."
    >
      <button type="button" class="btn btn-primary btn-sm" @click="openCreate">New work item</button>
    </EmptyState>

    <EmptyState
      v-else-if="filteredItems.length === 0"
      icon="bi-funnel"
      title="No work items match"
      message="Try another status or search term."
    />

    <div v-else class="table-responsive">
      <table class="table app-table table-hover">
        <thead>
          <tr>
            <th>Work item</th>
            <th>Task type</th>
            <th>Status</th>
            <th>Assignee</th>
            <th>Updated</th>
            <th class="text-end">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in filteredItems" :key="item.id">
            <td style="min-width: 240px">
              <div class="fw-medium">{{ item.title }}</div>
              <div v-if="item.description" class="small text-secondary text-clamp-2">{{ item.description }}</div>
            </td>
            <td>
              <div v-if="item.taskTypeName" class="d-flex align-items-center gap-2">
                <DifficultyBadge :level="item.difficulty" />
                <span class="small">{{ item.taskTypeName }}</span>
              </div>
              <span v-else class="small text-secondary">Not analyzed</span>
            </td>
            <td><StatusBadge :status="item.status" /></td>
            <td>
              <div v-if="item.assignedEmployeeName" class="d-flex align-items-center gap-2">
                <span class="avatar avatar-sm">{{ getInitials(item.assignedEmployeeName) }}</span>
                <span class="small text-nowrap">{{ item.assignedEmployeeName }}</span>
              </div>
              <span v-else class="small text-secondary">—</span>
            </td>
            <td class="small text-secondary text-nowrap" :title="formatDateTime(lastActivity(item))">
              {{ formatRelativeTime(lastActivity(item)) }}
            </td>
            <td>
              <div class="table-actions align-items-center">
                <button
                  v-if="item.status === 'New'"
                  type="button"
                  class="btn btn-sm btn-outline-primary workflow-action"
                  @click="analyzeTarget = item"
                >
                  <i class="bi bi-search me-1"></i>Analyze
                </button>

                <button
                  v-if="item.status === 'Analyzed'"
                  type="button"
                  class="btn btn-sm btn-primary workflow-action"
                  :disabled="busyItemId === item.id"
                  @click="assign(item)"
                >
                  <span v-if="busyItemId === item.id" class="spinner-border spinner-border-sm me-1"></span>
                  <i v-else class="bi bi-person-check me-1"></i>Assign
                </button>

                <button
                  v-if="item.status === 'Assigned'"
                  type="button"
                  class="btn btn-sm btn-outline-success workflow-action"
                  :disabled="busyItemId === item.id"
                  @click="complete(item)"
                >
                  <span v-if="busyItemId === item.id" class="spinner-border spinner-border-sm me-1"></span>
                  <i v-else class="bi bi-check2 me-1"></i>Complete
                </button>

                <button
                  v-if="item.status === 'Analyzed'"
                  type="button"
                  class="btn btn-sm"
                  title="Change task type"
                  aria-label="Change task type"
                  @click="analyzeTarget = item"
                >
                  <i class="bi bi-arrow-repeat"></i>
                </button>

                <button
                  v-if="item.status !== 'Completed'"
                  type="button"
                  class="btn btn-sm"
                  title="Edit"
                  :aria-label="`Edit ${item.title}`"
                  @click="openEdit(item)"
                >
                  <i class="bi bi-pencil"></i>
                </button>

                <button
                  v-if="item.status === 'New' || item.status === 'Analyzed'"
                  type="button"
                  class="btn btn-sm btn-delete"
                  title="Delete"
                  :aria-label="`Delete ${item.title}`"
                  @click="deleteTarget = item"
                >
                  <i class="bi bi-trash3"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>

  <WorkItemFormModal
    :show="isFormOpen"
    :work-item="editingItem"
    @close="isFormOpen = false"
    @saved="handleSaved"
  />

  <AnalyzeWorkItemModal
    :show="analyzeTarget !== null"
    :work-item="analyzeTarget"
    :task-types="taskTypes"
    @close="analyzeTarget = null"
    @analyzed="handleAnalyzed"
    @assigned="handleAssigned"
  />

  <AssignmentResultModal
    :show="assignmentResult !== null"
    :result="assignmentResult"
    @close="assignmentResult = null"
  />

  <ConfirmDialog
    :show="deleteTarget !== null"
    title="Delete work item"
    :message="`Delete &quot;${deleteTarget?.title}&quot;? This cannot be undone.`"
    confirm-label="Delete"
    :loading="isDeleting"
    @confirm="confirmDelete"
    @cancel="deleteTarget = null"
  />
</template>