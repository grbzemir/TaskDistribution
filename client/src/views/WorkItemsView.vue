<script setup>
import PageHeader from '@/components/common/PageHeader.vue'
import EmptyState from '@/components/common/EmptyState.vue'
</script>

<template>
  <PageHeader title="Work items" subtitle="Create, analyze and assign tasks." />
  <div class="surface">
    <EmptyState icon="bi-kanban" title="Coming soon" message="This screen will be implemented next." />
  </div>
</template>