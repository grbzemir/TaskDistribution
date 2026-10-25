<script setup>
import { computed, reactive, ref, watch } from 'vue'
import { workItemsApi } from '@/api'
import AppModal from '@/components/common/AppModal.vue'

const props = defineProps({
  show: { type: Boolean, default: false },
  workItem: { type: Object, default: null },
})

const emit = defineEmits(['close', 'saved'])

const form = reactive({
  title: '',
  description: '',
})

const errors = ref({})
const generalError = ref('')
const isSaving = ref(false)

const isEditMode = computed(() => props.workItem !== null)

watch(
  () => props.show,
  (isOpen) => {
    if (!isOpen) {
      return
    }

    form.title = props.workItem?.title ?? ''
    form.description = props.workItem?.description ?? ''
    errors.value = {}
    generalError.value = ''
  },
)

function validate() {
  const result = {}

  if (!form.title.trim()) result.title = ['Title is required.']

  errors.value = result
  return Object.keys(result).length === 0
}

async function submit() {
  if (isSaving.value || !validate()) {
    return
  }

  isSaving.value = true
  generalError.value = ''

  const payload = {
    title: form.title.trim(),
    description: form.description.trim() || null,
  }

  try {
    const saved = isEditMode.value
      ? await workItemsApi.update(props.workItem.id, payload)
      : await workItemsApi.create(payload)

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
  <AppModal :show="show" :title="isEditMode ? 'Edit work item' : 'New work item'" @close="close">
    <form id="work-item-form" novalidate @submit.prevent="submit">
      <div v-if="generalError" class="alert alert-danger py-2 small">{{ generalError }}</div>

      <div class="mb-3">
        <label for="work-item-title" class="form-label">Title</label>
        <input
          id="work-item-title"
          v-model="form.title"
          type="text"
          class="form-control"
          :class="{ 'is-invalid': errors.title }"
          maxlength="200"
          autocomplete="off"
        />
        <div v-if="errors.title" class="invalid-feedback">{{ errors.title[0] }}</div>
      </div>

      <div>
        <label for="work-item-description" class="form-label">
          Description <span class="text-secondary fw-normal">(optional)</span>
        </label>
        <textarea
          id="work-item-description"
          v-model="form.description"
          class="form-control"
          :class="{ 'is-invalid': errors.description }"
          rows="4"
          maxlength="2000"
        ></textarea>
        <div v-if="errors.description" class="invalid-feedback">{{ errors.description[0] }}</div>
      </div>

      <div v-if="!isEditMode" class="form-text mt-2">
        After creating the work item, an analyst sets its difficulty by choosing a task type.
      </div>
    </form>

    <template #footer>
      <button type="button" class="btn btn-light" :disabled="isSaving" @click="close">Cancel</button>
      <button type="submit" form="work-item-form" class="btn btn-primary" :disabled="isSaving">
        <span v-if="isSaving" class="spinner-border spinner-border-sm me-2"></span>
        {{ isEditMode ? 'Save changes' : 'Create work item' }}
      </button>
    </template>
  </AppModal>
</template>