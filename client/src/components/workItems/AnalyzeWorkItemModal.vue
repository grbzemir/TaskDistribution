<script setup>
import { computed, ref, watch } from 'vue'
import { workItemsApi } from '@/api'
import { useToastStore } from '@/stores/toast'
import AppModal from '@/components/common/AppModal.vue'
import DifficultyBadge from '@/components/common/DifficultyBadge.vue'

const props = defineProps({
  show: { type: Boolean, default: false },
  workItem: { type: Object, default: null },
  taskTypes: { type: Array, default: () => [] },
})

const emit = defineEmits(['close', 'analyzed', 'assigned'])

const toast = useToastStore()

const selectedTaskTypeId = ref(null)
const generalError = ref('')
const pendingAction = ref(null)

const isBusy = computed(() => pendingAction.value !== null)

const sortedTaskTypes = computed(() =>
  [...props.taskTypes].sort((a, b) => a.difficulty - b.difficulty),
)

watch(
  () => props.show,
  (isOpen) => {
    if (!isOpen) {
      return
    }

    selectedTaskTypeId.value = props.workItem?.taskTypeId ?? null
    generalError.value = ''
  },
)

async function submit(withAssignment) {
  if (isBusy.value) {
    return
  }

  if (!selectedTaskTypeId.value) {
    generalError.value = 'Select a task type to set the difficulty.'
    return
  }

  pendingAction.value = withAssignment ? 'assign' : 'analyze'
  generalError.value = ''

  try {
    const analyzed = await workItemsApi.analyze(props.workItem.id, selectedTaskTypeId.value)

    if (!withAssignment) {
      emit('analyzed', analyzed)
      return
    }

    try {
      emit('assigned', await workItemsApi.assign(analyzed.id))
    } catch (error) {
      emit('analyzed', analyzed)
      toast.error(`The analysis was saved, but the assignment failed: ${error.message}`)
    }
  } catch (error) {
    generalError.value = error.message
  } finally {
    pendingAction.value = null
  }
}

function close() {
  if (!isBusy.value) {
    emit('close')
  }
}
</script>

<template>
  <AppModal :show="show" title="Analyze work item" size="lg" @close="close">
    <p class="small text-secondary mb-1">Work item</p>
    <p class="fw-semibold mb-3">{{ workItem?.title }}</p>

    <div v-if="generalError" class="alert alert-danger py-2 small">{{ generalError }}</div>

    <label class="form-label">Task type</label>
    <div class="task-type-options" role="radiogroup" aria-label="Task type">
      <label
        v-for="taskType in sortedTaskTypes"
        :key="taskType.id"
        class="task-type-option"
        :class="{ selected: selectedTaskTypeId === taskType.id }"
      >
        <input
          v-model="selectedTaskTypeId"
          type="radio"
          class="visually-hidden"
          name="task-type"
          :value="taskType.id"
          :disabled="isBusy"
        />
        <DifficultyBadge :level="taskType.difficulty" />
        <span class="task-type-option-text">
          <span class="fw-medium d-block">{{ taskType.name }}</span>
          <span v-if="taskType.description" class="small text-secondary">{{ taskType.description }}</span>
        </span>
      </label>
    </div>
    <div class="form-text mt-2">
      The difficulty of the selected task type is copied to the work item and used for fair assignment.
    </div>

    <template #footer>
      <button type="button" class="btn btn-light" :disabled="isBusy" @click="close">Cancel</button>
      <button type="button" class="btn btn-outline-primary" :disabled="isBusy" @click="submit(false)">
        <span v-if="pendingAction === 'analyze'" class="spinner-border spinner-border-sm me-2"></span>
        Save analysis
      </button>
      <button type="button" class="btn btn-primary" :disabled="isBusy" @click="submit(true)">
        <span v-if="pendingAction === 'assign'" class="spinner-border spinner-border-sm me-2"></span>
        <i v-else class="bi bi-lightning-charge me-1"></i>
        Analyze and assign
      </button>
    </template>
  </AppModal>
</template>