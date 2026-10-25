<script setup>
import { computed, onMounted, ref } from 'vue'
import { taskTypesApi } from '@/api'
import { useToastStore } from '@/stores/toast'
import { DIFFICULTY_LEVELS } from '@/utils/difficulty'
import PageHeader from '@/components/common/PageHeader.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ConfirmDialog from '@/components/common/ConfirmDialog.vue'
import DifficultyBadge from '@/components/common/DifficultyBadge.vue'
import DifficultyMeter from '@/components/common/DifficultyMeter.vue'
import TaskTypeFormModal from '@/components/taskTypes/TaskTypeFormModal.vue'

const toast = useToastStore()

const taskTypes = ref([])
const isLoading = ref(true)
const loadError = ref('')

const isFormOpen = ref(false)
const editingTaskType = ref(null)

const deleteTarget = ref(null)
const isDeleting = ref(false)

const freeLevelCount = computed(() => DIFFICULTY_LEVELS.length - taskTypes.value.length)

function sortByDifficulty(list) {
  return [...list].sort((a, b) => a.difficulty - b.difficulty)
}

async function loadData() {
  isLoading.value = true
  loadError.value = ''

  try {
    taskTypes.value = sortByDifficulty(await taskTypesApi.getAll())
  } catch (error) {
    loadError.value = error.message
  } finally {
    isLoading.value = false
  }
}

function openCreate() {
  editingTaskType.value = null
  isFormOpen.value = true
}

function openEdit(taskType) {
  editingTaskType.value = taskType
  isFormOpen.value = true
}

function handleSaved(saved) {
  if (editingTaskType.value) {
    taskTypes.value = sortByDifficulty(taskTypes.value.map((t) => (t.id === saved.id ? saved : t)))
    toast.success(`${saved.name} was updated.`)
  } else {
    taskTypes.value = sortByDifficulty([...taskTypes.value, saved])
    toast.success(`${saved.name} was added.`)
  }

  isFormOpen.value = false
}

async function confirmDelete() {
  const target = deleteTarget.value
  isDeleting.value = true

  try {
    await taskTypesApi.remove(target.id)
    taskTypes.value = taskTypes.value.filter((t) => t.id !== target.id)
    toast.success(`${target.name} was deleted.`)
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
  <PageHeader
    title="Task types"
    :subtitle="`${taskTypes.length} task types · ${freeLevelCount} free difficulty levels`"
  >
    <template #actions>
      <button type="button" class="btn btn-primary" @click="openCreate">
        <i class="bi bi-plus-lg me-1"></i> New task type
      </button>
    </template>
  </PageHeader>

  <div class="info-callout">
    <i class="bi bi-lightbulb"></i>
    <span>
      When analysts analyze a work item, they pick one of these task types. Its difficulty is copied to the
      work item and drives the fair distribution across developers.
    </span>
  </div>

  <div class="surface">
    <LoadingState v-if="isLoading" label="Loading task types…" />

    <EmptyState v-else-if="loadError" icon="bi-exclamation-triangle" title="Couldn't load task types" :message="loadError">
      <button type="button" class="btn btn-outline-primary btn-sm" @click="loadData">Try again</button>
    </EmptyState>

    <EmptyState
      v-else-if="taskTypes.length === 0"
      icon="bi-layers"
      title="No task types yet"
      message="Create task types so analysts can classify work items."
    />

    <div v-else class="table-responsive">
      <table class="table app-table table-hover">
        <thead>
          <tr>
            <th style="width: 90px">Level</th>
            <th>Task type</th>
            <th style="width: 160px">Difficulty</th>
            <th style="width: 180px">Added</th>
            <th class="text-end" style="width: 100px">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="taskType in taskTypes" :key="taskType.id">
            <td><DifficultyBadge :level="taskType.difficulty" /></td>
            <td>
              <div class="fw-medium">{{ taskType.name }}</div>
              <div v-if="taskType.description" class="small text-secondary">{{ taskType.description }}</div>
            </td>
            <td><DifficultyMeter :level="taskType.difficulty" /></td>
            <td class="text-secondary small">{{ new Date(taskType.createdAt).toLocaleDateString('en-GB') }}</td>
            <td>
              <div class="table-actions">
                <button
                  type="button"
                  class="btn btn-sm"
                  :aria-label="`Edit ${taskType.name}`"
                  title="Edit"
                  @click="openEdit(taskType)"
                >
                  <i class="bi bi-pencil"></i>
                </button>
                <button
                  type="button"
                  class="btn btn-sm btn-delete"
                  :aria-label="`Delete ${taskType.name}`"
                  title="Delete"
                  @click="deleteTarget = taskType"
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

  <TaskTypeFormModal
    :show="isFormOpen"
    :task-type="editingTaskType"
    :task-types="taskTypes"
    @close="isFormOpen = false"
    @saved="handleSaved"
  />

  <ConfirmDialog
    :show="deleteTarget !== null"
    title="Delete task type"
    :message="`Delete ${deleteTarget?.name}? Task types that are used by work items can't be deleted.`"
    confirm-label="Delete"
    :loading="isDeleting"
    @confirm="confirmDelete"
    @cancel="deleteTarget = null"
  />
</template>