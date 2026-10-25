<script setup>
import { computed, onMounted, ref } from 'vue'
import { employeesApi, rolesApi } from '@/api'
import { useToastStore } from '@/stores/toast'
import { formatDateTime } from '@/utils/formatters'
import PageHeader from '@/components/common/PageHeader.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ConfirmDialog from '@/components/common/ConfirmDialog.vue'
import EmployeeFormModal from '@/components/employees/EmployeeFormModal.vue'

const toast = useToastStore()

const employees = ref([])
const roles = ref([])
const isLoading = ref(true)
const loadError = ref('')

const search = ref('')
const roleFilter = ref('')
const statusFilter = ref('all')

const isFormOpen = ref(false)
const editingEmployee = ref(null)

const deleteTarget = ref(null)
const isDeleting = ref(false)

const deactivateTarget = ref(null)
const isDeactivating = ref(false)

const filteredEmployees = computed(() => {
  const term = search.value.trim().toLocaleLowerCase('tr')

  return employees.value.filter((employee) => {
    const matchesSearch = !term || employee.fullName.toLocaleLowerCase('tr').includes(term)
    const matchesRole = !roleFilter.value || employee.roleId === roleFilter.value
    const matchesStatus =
      statusFilter.value === 'all' ||
      (statusFilter.value === 'active' && employee.isActive) ||
      (statusFilter.value === 'inactive' && !employee.isActive)

    return matchesSearch && matchesRole && matchesStatus
  })
})

const activeDeveloperCount = computed(
  () => employees.value.filter((e) => e.isActive && e.roleName === 'Developer').length,
)

async function loadData() {
  isLoading.value = true
  loadError.value = ''

  try {
    const [employeeList, roleList] = await Promise.all([employeesApi.getAll(), rolesApi.getAll()])
    employees.value = employeeList
    roles.value = roleList
  } catch (error) {
    loadError.value = error.message
  } finally {
    isLoading.value = false
  }
}

function initials(employee) {
  return `${employee.firstName.charAt(0)}${employee.lastName.charAt(0)}`.toLocaleUpperCase('tr')
}

function sortByName(list) {
  return [...list].sort((a, b) => a.fullName.localeCompare(b.fullName, 'tr'))
}

function replaceEmployee(updated) {
  employees.value = sortByName(employees.value.map((e) => (e.id === updated.id ? updated : e)))
}

function openCreate() {
  editingEmployee.value = null
  isFormOpen.value = true
}

function openEdit(employee) {
  editingEmployee.value = employee
  isFormOpen.value = true
}

function handleSaved(saved) {
  if (editingEmployee.value) {
    replaceEmployee(saved)
    toast.success(`${saved.fullName} was updated.`)
  } else {
    employees.value = sortByName([...employees.value, saved])
    toast.success(`${saved.fullName} was added.`)
  }

  isFormOpen.value = false
}

async function confirmDelete() {
  const target = deleteTarget.value
  isDeleting.value = true

  try {
    await employeesApi.remove(target.id)
    employees.value = employees.value.filter((e) => e.id !== target.id)
    toast.success(`${target.fullName} was deleted.`)
    deleteTarget.value = null
  } catch (error) {
    deleteTarget.value = null

    if (error.status === 409 && target.isActive) {
      deactivateTarget.value = target
    } else if (error.status === 409) {
      toast.info(`${target.fullName} is already inactive and is kept for assignment history.`)
    } else {
      toast.error(error.message)
    }
  } finally {
    isDeleting.value = false
  }
}

async function confirmDeactivate() {
  const target = deactivateTarget.value
  isDeactivating.value = true

  try {
    const updated = await employeesApi.update(target.id, {
      firstName: target.firstName,
      lastName: target.lastName,
      roleId: target.roleId,
      isActive: false,
    })

    replaceEmployee(updated)
    toast.success(`${updated.fullName} was deactivated.`)
    deactivateTarget.value = null
  } catch (error) {
    toast.error(error.message)
  } finally {
    isDeactivating.value = false
  }
}

onMounted(loadData)
</script>

<template>
  <PageHeader
    title="Employees"
    :subtitle="`${employees.length} people · ${activeDeveloperCount} active developers in the assignment pool`"
  >
    <template #actions>
      <button type="button" class="btn btn-primary" @click="openCreate">
        <i class="bi bi-plus-lg me-1"></i> New employee
      </button>
    </template>
  </PageHeader>

  <div class="surface">
    <div class="toolbar">
      <div class="search-input">
        <i class="bi bi-search"></i>
        <input v-model="search" type="search" class="form-control" placeholder="Search by name" />
      </div>

      <select v-model="roleFilter" class="form-select" aria-label="Filter by role">
        <option value="">All roles</option>
        <option v-for="role in roles" :key="role.id" :value="role.id">{{ role.name }}</option>
      </select>

      <select v-model="statusFilter" class="form-select" aria-label="Filter by status">
        <option value="all">All statuses</option>
        <option value="active">Active</option>
        <option value="inactive">Inactive</option>
      </select>

      <span class="ms-auto small text-secondary">
        Showing {{ filteredEmployees.length }} of {{ employees.length }}
      </span>
    </div>

    <LoadingState v-if="isLoading" label="Loading employees…" />

    <EmptyState v-else-if="loadError" icon="bi-exclamation-triangle" title="Couldn't load employees" :message="loadError">
      <button type="button" class="btn btn-outline-primary btn-sm" @click="loadData">Try again</button>
    </EmptyState>

    <EmptyState
      v-else-if="filteredEmployees.length === 0"
      icon="bi-person-x"
      title="No employees found"
      message="Try a different search or filter."
    />

    <div v-else class="table-responsive">
      <table class="table app-table table-hover">
        <thead>
          <tr>
            <th>Name</th>
            <th>Role</th>
            <th>Status</th>
            <th>Added</th>
            <th class="text-end">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="employee in filteredEmployees"
            :key="employee.id"
            :class="{ 'row-inactive': !employee.isActive }"
          >
            <td>
              <div class="d-flex align-items-center gap-2">
                <span class="avatar" :class="{ 'avatar-muted': !employee.isActive }">
                  {{ initials(employee) }}
                </span>
                <span class="fw-medium">{{ employee.fullName }}</span>
              </div>
            </td>
            <td>
              <span class="role-badge" :class="`role-${employee.roleName.toLowerCase()}`">
                {{ employee.roleName }}
              </span>
            </td>
            <td>
              <span class="activity-status" :class="employee.isActive ? 'activity-active' : 'activity-inactive'">
                {{ employee.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="text-secondary small">{{ formatDateTime(employee.createdAt) }}</td>
            <td>
              <div class="table-actions">
                <button
                  type="button"
                  class="btn btn-sm"
                  :aria-label="`Edit ${employee.fullName}`"
                  title="Edit"
                  @click="openEdit(employee)"
                >
                  <i class="bi bi-pencil"></i>
                </button>
                <button
                  type="button"
                  class="btn btn-sm btn-delete"
                  :aria-label="`Delete ${employee.fullName}`"
                  title="Delete"
                  @click="deleteTarget = employee"
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

  <EmployeeFormModal
    :show="isFormOpen"
    :employee="editingEmployee"
    :roles="roles"
    @close="isFormOpen = false"
    @saved="handleSaved"
  />

  <ConfirmDialog
    :show="deleteTarget !== null"
    title="Delete employee"
    :message="`Delete ${deleteTarget?.fullName}? This cannot be undone.`"
    confirm-label="Delete"
    :loading="isDeleting"
    @confirm="confirmDelete"
    @cancel="deleteTarget = null"
  />

  <ConfirmDialog
    :show="deactivateTarget !== null"
    title="Employee has assigned work items"
    :message="`${deactivateTarget?.fullName} has work items in the distribution history, so the record can't be deleted. Deactivate instead? Inactive employees are excluded from automatic assignment.`"
    confirm-label="Deactivate"
    variant="warning"
    :loading="isDeactivating"
    @confirm="confirmDeactivate"
    @cancel="deactivateTarget = null"
  />
</template>