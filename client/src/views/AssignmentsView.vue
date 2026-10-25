<script setup>
import PageHeader from '@/components/common/PageHeader.vue'
import EmptyState from '@/components/common/EmptyState.vue'
</script>

<template>
  <PageHeader title="Assignments" subtitle="Assignment history and active distribution rules." />
  <div class="surface">
    <EmptyState icon="bi-diagram-3" title="Coming soon" message="This screen will be implemented next." />
  </div>
</template>