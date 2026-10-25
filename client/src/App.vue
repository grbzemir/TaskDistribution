<script setup>
import { ref, watch } from 'vue'
import { RouterView, useRoute } from 'vue-router'
import AppSidebar from '@/components/layout/AppSidebar.vue'
import ToastContainer from '@/components/common/ToastContainer.vue'

const route = useRoute()
const isSidebarOpen = ref(false)

watch(
  () => route.fullPath,
  () => {
    isSidebarOpen.value = false
  },
)
</script>

<template>
  <div class="app-shell">
    <AppSidebar :open="isSidebarOpen" @close="isSidebarOpen = false" />

    <div class="app-main">
      <header class="app-topbar d-lg-none">
        <button type="button" class="btn-icon" aria-label="Open menu" @click="isSidebarOpen = true">
          <i class="bi bi-list"></i>
        </button>
        <span class="fw-semibold">TaskFlow</span>
      </header>

      <main class="app-content">
        <RouterView />
      </main>
    </div>

    <ToastContainer />
  </div>
</template>