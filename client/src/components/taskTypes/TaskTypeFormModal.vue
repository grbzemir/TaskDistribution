<script setup>
import { computed, reactive, ref, watch } from 'vue'
import { taskTypesApi } from '@/api'
import { DIFFICULTY_LEVELS } from '@/utils/difficulty'
import AppModal from '@/components/common/AppModal.vue'
import DifficultyPicker from './DifficultyPicker.vue'

const props = defineProps({
  show: { type: Boolean, default: false },
  taskType: { type: Object, default: null },
  taskTypes: { type: Array, default: () => [] },
})

const emit = defineEmits(['close', 'saved'])

const form = reactive({
  name: '',
  description: '',
  difficulty: null,
})

const errors = ref({})
const generalError = ref('')
const isSaving = ref(false)

const isEditMode = computed(() => props.taskType !== null)

const takenLevels = computed(() =>
  props.taskTypes.filter((t) => t.id !== props.taskType?.id).map((t) => t.difficulty),
)

const allLevelsTaken = computed(
  () => !isEditMode.value && takenLevels.value.length >= DIFFICULTY_LEVELS.length,
)

watch(
  () => props.show,
  (isOpen) => {
    if (!isOpen) {
      return
    }

    form.name = props.taskType?.name ?? ''
    form.description = props.taskType?.description ?? ''
    form.difficulty = props.taskType?.difficulty ?? null
    errors.value = {}
    generalError.value = ''
  },
)

function validate() {
  const result = {}

  if (!form.name.trim()) result.name = ['Name is required.']
  if (form.difficulty == null) result.difficulty = ['Select a difficulty level.']

  errors.value = result
  return Object.keys(result).length === 0
}

async function submit() {
  if (isSaving.value || allLevelsTaken.value || !validate()) {
    return
  }

  isSaving.value = true
  generalError.value = ''

  const payload = {
    name: form.name.trim(),
    description: form.description.trim() || null,
    difficulty: form.difficulty,
  }

  try {
    const saved = isEditMode.value
      ? await taskTypesApi.update(props.taskType.id, payload)
      : await taskTypesApi.create(payload)

    emit('saved', saved)
  } catch (error) {
    errors.value = error.fieldErrors ?? {}

    if (Object.keys(errors.value).length === 0) {
      generalError.value = error.message
    }
  } finally {
    isSaving.value = false
  }
}

function close() {
  if (!isSaving.value) {
    emit('close')
  }
}
</script>

<template>
  <AppModal :show="show" :title="isEditMode ? 'Edit task type' : 'New task type'" @close="close">
    <div v-if="allLevelsTaken" class="info-callout">
      <i class="bi bi-info-circle"></i>
      <span>
        All {{ DIFFICULTY_LEVELS.length }} difficulty levels are already in use. Each task type needs a
        unique difficulty, so edit or delete an existing task type to free up a level.
      </span>
    </div>

    <form id="task-type-form" novalidate @submit.prevent="submit">
      <div v-if="generalError" class="alert alert-danger py-2 small">{{ generalError }}</div>

      <div class="mb-3">
        <label for="task-type-name" class="form-label">Name</label>
        <input
          id="task-type-name"
          v-model="form.name"
          type="text"
          class="form-control"
          :class="{ 'is-invalid': errors.name }"
          maxlength="100"
          autocomplete="off"
          :disabled="allLevelsTaken"
        />
        <div v-if="errors.name" class="invalid-feedback">{{ errors.name[0] }}</div>
      </div>

      <div class="mb-3">
        <label for="task-type-description" class="form-label">
          Description <span class="text-secondary fw-normal">(optional)</span>
        </label>
        <textarea
          id="task-type-description"
          v-model="form.description"
          class="form-control"
          :class="{ 'is-invalid': errors.description }"
          rows="2"
          maxlength="500"
          :disabled="allLevelsTaken"
        ></textarea>
        <div v-if="errors.description" class="invalid-feedback">{{ errors.description[0] }}</div>
      </div>

      <div>
        <label class="form-label">Difficulty</label>
        <DifficultyPicker
          v-model="form.difficulty"
          :taken-levels="takenLevels"
          :invalid="Boolean(errors.difficulty)"
        />
        <div v-if="errors.difficulty" class="invalid-feedback d-block">{{ errors.difficulty[0] }}</div>
        <div v-if="isEditMode" class="form-text">
          Changing the difficulty only affects work items analyzed from now on. Existing work items keep the
          difficulty they were analyzed with.
        </div>
      </div>
    </form>

    <template #footer>
      <button type="button" class="btn btn-light" :disabled="isSaving" @click="close">Cancel</button>
      <button
        type="submit"
        form="task-type-form"
        class="btn btn-primary"
        :disabled="isSaving || allLevelsTaken"
      >
        <span v-if="isSaving" class="spinner-border spinner-border-sm me-2"></span>
        {{ isEditMode ? 'Save changes' : 'Create task type' }}
      </button>
    </template>
  </AppModal>
</template>