<script setup>
import { RouterLink } from 'vue-router'

defineProps({
  open: { type: Boolean, default: false },
})

defineEmits(['close'])

const navigation = [
  { to: '/', label: 'Dashboard', icon: 'bi-grid-1x2' },
  { to: '/work-items', label: 'Work items', icon: 'bi-kanban' },
  { to: '/assignments', label: 'Assignments', icon: 'bi-diagram-3' },
]

const management = [
  { to: '/employees', label: 'Employees', icon: 'bi-people' },
  { to: '/task-types', label: 'Task types', icon: 'bi-layers' },
]
</script>

<template>
  <div v-if="open" class="sidebar-backdrop d-lg-none" @click="$emit('close')"></div>

  <aside class="app-sidebar" :class="{ 'is-open': open }">
    <div class="sidebar-brand">
      <span class="brand-mark"><i class="bi bi-shuffle"></i></span>
      <div>
        <div class="brand-name">TaskFlow</div>
        <div class="brand-caption">Fair task distribution</div>
      </div>
    </div>

    <div class="sidebar-section-label">Workflow</div>
    <nav class="sidebar-nav mb-4">
      <RouterLink v-for="item in navigation" :key="item.to" :to="item.to" class="sidebar-link">
        <i class="bi" :class="item.icon"></i>
        <span>{{ item.label }}</span>
      </RouterLink>
    </nav>

    <div class="sidebar-section-label">Management</div>
    <nav class="sidebar-nav">
      <RouterLink v-for="item in management" :key="item.to" :to="item.to" class="sidebar-link">
        <i class="bi" :class="item.icon"></i>
        <span>{{ item.label }}</span>
      </RouterLink>
    </nav>

    <div class="sidebar-footer">v1.0 · .NET 10 + Vue 3</div>
  </aside>
</template>