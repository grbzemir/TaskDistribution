<script setup>
import { computed, reactive, ref, watch } from 'vue'
import { employeesApi } from '@/api'
import AppModal from '@/components/common/AppModal.vue'

const props = defineProps({
  show: { type: Boolean, default: false },
  employee: { type: Object, default: null },
  roles: { type: Array, default: () => [] },
})

const emit = defineEmits(['close', 'saved'])

const form = reactive({
  firstName: '',
  lastName: '',
  roleId: '',
  isActive: true,
})

const errors = ref({})
const generalError = ref('')
const isSaving = ref(false)

const isEditMode = computed(() => props.employee !== null)

watch(
  () => props.show,
  (isOpen) => {
    if (!isOpen) {
      return
    }

    form.firstName = props.employee?.firstName ?? ''
    form.lastName = props.employee?.lastName ?? ''
    form.roleId = props.employee?.roleId ?? ''
    form.isActive = props.employee?.isActive ?? true
    errors.value = {}
    generalError.value = ''
  },
)

function validate() {
  const result = {}

  if (!form.firstName.trim()) result.firstName = ['First name is required.']
  if (!form.lastName.trim()) result.lastName = ['Last name is required.']
  if (!form.roleId) result.roleId = ['Select a role.']

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
    firstName: form.firstName.trim(),
    lastName: form.lastName.trim(),
    roleId: Number(form.roleId),
  }

  try {
    const saved = isEditMode.value
      ? await employeesApi.update(props.employee.id, { ...payload, isActive: form.isActive })
      : await employeesApi.create(payload)

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
  <AppModal :show="show" :title="isEditMode ? 'Edit employee' : 'New employee'" @close="close">
    <form id="employee-form" novalidate @submit.prevent="submit">
      <div v-if="generalError" class="alert alert-danger py-2 small">{{ generalError }}</div>

      <div class="row g-3">
        <div class="col-sm-6">
          <label for="employee-first-name" class="form-label">First name</label>
          <input
            id="employee-first-name"
            v-model="form.firstName"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.firstName }"
            maxlength="50"
            autocomplete="off"
          />
          <div v-if="errors.firstName" class="invalid-feedback">{{ errors.firstName[0] }}</div>
        </div>

        <div class="col-sm-6">
          <label for="employee-last-name" class="form-label">Last name</label>
          <input
            id="employee-last-name"
            v-model="form.lastName"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.lastName }"
            maxlength="50"
            autocomplete="off"
          />
          <div v-if="errors.lastName" class="invalid-feedback">{{ errors.lastName[0] }}</div>
        </div>

        <div class="col-12">
          <label for="employee-role" class="form-label">Role</label>
          <select
            id="employee-role"
            v-model="form.roleId"
            class="form-select"
            :class="{ 'is-invalid': errors.roleId }"
          >
            <option value="" disabled>Select a role</option>
            <option v-for="role in roles" :key="role.id" :value="role.id">{{ role.name }}</option>
          </select>
          <div v-if="errors.roleId" class="invalid-feedback">{{ errors.roleId[0] }}</div>
          <div class="form-text">Only active developers are included in automatic task assignment.</div>
        </div>

        <div v-if="isEditMode" class="col-12">
          <div class="form-check form-switch">
            <input
              id="employee-active"
              v-model="form.isActive"
              class="form-check-input"
              type="checkbox"
              role="switch"
            />
            <label class="form-check-label" for="employee-active">Active</label>
          </div>
        </div>
      </div>
    </form>

    <template #footer>
      <button type="button" class="btn btn-light" :disabled="isSaving" @click="close">Cancel</button>
      <button type="submit" form="employee-form" class="btn btn-primary" :disabled="isSaving">
        <span v-if="isSaving" class="spinner-border spinner-border-sm me-2"></span>
        {{ isEditMode ? 'Save changes' : 'Create employee' }}
      </button>
    </template>
  </AppModal>
</template>