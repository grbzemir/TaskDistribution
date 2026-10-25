<script setup>
import PageHeader from '@/components/common/PageHeader.vue'
import EmptyState from '@/components/common/EmptyState.vue'
</script>

<template>
  <PageHeader title="Employees" subtitle="Manage developers, analysts and managers." />
  <div class="surface">
    <EmptyState icon="bi-people" title="Coming soon" message="This screen will be implemented next." />
  </div>
</template>