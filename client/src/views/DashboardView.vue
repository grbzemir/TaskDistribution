<script setup>
import { onMounted, ref } from 'vue'
import { employeesApi, taskTypesApi } from '@/api'
import PageHeader from '@/components/common/PageHeader.vue'
import LoadingState from '@/components/common/LoadingState.vue'
import EmptyState from '@/components/common/EmptyState.vue'

const state = ref('loading')
const counts = ref({ employees: 0, taskTypes: 0 })
const errorMessage = ref('')

onMounted(async () => {
  try {
    const [employees, taskTypes] = await Promise.all([employeesApi.getAll(), taskTypesApi.getAll()])
    counts.value = { employees: employees.length, taskTypes: taskTypes.length }
    state.value = 'ready'
  } catch (error) {
    errorMessage.value = error.message
    state.value = 'error'
  }
})
</script>

<template>
  <PageHeader title="Dashboard" subtitle="Overview of workload distribution across the team." />

  <div class="surface">
    <LoadingState v-if="state === 'loading'" label="Connecting to the API…" />

    <EmptyState
      v-else-if="state === 'error'"
      icon="bi-plug"
      title="API connection failed"
      :message="errorMessage"
    />

    <EmptyState
      v-else
      icon="bi-check2-circle"
      title="API connection is working"
      :message="`Loaded ${counts.employees} employees and ${counts.taskTypes} task types.`"
    />
  </div>
</template>