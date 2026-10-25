<script setup>
import AppModal from '@/components/common/AppModal.vue'
import DifficultyBadge from '@/components/common/DifficultyBadge.vue'
import { getRuleLabel } from '@/utils/assignmentRules'
import { getInitials } from '@/utils/formatters'

defineProps({
  show: { type: Boolean, default: false },
  result: { type: Object, default: null },
})

const emit = defineEmits(['close'])
</script>

<template>
  <AppModal :show="show" title="Work item assigned" @close="emit('close')">
    <template v-if="result">
      <div class="assignment-hero">
        <span class="avatar avatar-lg">{{ getInitials(result.assignedEmployeeName) }}</span>
        <div>
          <div class="small text-secondary">Assigned to</div>
          <div class="fs-5 fw-semibold">{{ result.assignedEmployeeName }}</div>
        </div>
      </div>

      <div class="d-flex align-items-center gap-2 mb-3">
        <DifficultyBadge :level="result.workItem.difficulty" />
        <span class="fw-medium">{{ result.workItem.title }}</span>
      </div>

      <div v-if="result.isRuleRelaxed" class="warning-callout">
        <i class="bi bi-exclamation-triangle"></i>
        <span>
          No developer satisfied every active rule, so the rules were relaxed and the fairest developer was
          selected.
        </span>
      </div>

      <div class="reason-box">
        <div class="reason-label"><i class="bi bi-chat-square-text"></i> Why this developer?</div>
        <p class="mb-0">{{ result.reason }}</p>
      </div>

      <div class="mt-3">
        <div class="small text-secondary mb-2">Active rules</div>
        <div class="d-flex flex-wrap gap-2">
          <span v-for="rule in result.appliedRules" :key="rule" class="rule-chip">
            <i class="bi bi-shield-check"></i>
            {{ getRuleLabel(rule) }}
          </span>
          <span v-if="result.appliedRules.length === 0" class="small text-secondary">
            No rules are enabled. Only the fairness ordering was applied.
          </span>
        </div>
      </div>
    </template>

    <template #footer>
      <button type="button" class="btn btn-primary" @click="emit('close')">Done</button>
    </template>
  </AppModal>
</template>