<script setup>
import { onBeforeUnmount, watch } from 'vue'

const props = defineProps({
  show: { type: Boolean, default: false },
  title: { type: String, default: '' },
  size: { type: String, default: '' },
})

const emit = defineEmits(['close'])

function close() {
  emit('close')
}

function handleKeydown(event) {
  if (event.key === 'Escape') {
    close()
  }
}

watch(
  () => props.show,
  (isOpen) => {
    document.body.classList.toggle('modal-open', isOpen)

    if (isOpen) {
      document.addEventListener('keydown', handleKeydown)
    } else {
      document.removeEventListener('keydown', handleKeydown)
    }
  },
  { immediate: true },
)

onBeforeUnmount(() => {
  document.body.classList.remove('modal-open')
  document.removeEventListener('keydown', handleKeydown)
})
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="show" class="app-modal-root">
        <div class="modal-backdrop show"></div>
        <div class="modal d-block" tabindex="-1" role="dialog" aria-modal="true" @mousedown.self="close">
          <div class="modal-dialog modal-dialog-centered" :class="size ? `modal-${size}` : ''">
            <div class="modal-content app-modal">
              <div class="modal-header">
                <h2 class="modal-title fs-6 fw-semibold">{{ title }}</h2>
                <button type="button" class="btn-close" aria-label="Close" @click="close"></button>
              </div>
              <div class="modal-body">
                <slot />
              </div>
              <div v-if="$slots.footer" class="modal-footer">
                <slot name="footer" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>