<script setup>
import { RouterLink } from 'vue-router'
import EmptyState from '@/components/common/EmptyState.vue'
</script>

<template>
  <div class="surface mt-4">
    <EmptyState icon="bi-signpost-split" title="Page not found" message="The page you are looking for does not exist.">
      <RouterLink to="/" class="btn btn-primary btn-sm">Go to dashboard</RouterLink>
    </EmptyState>
  </div>
</template>