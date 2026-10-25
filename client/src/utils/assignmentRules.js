const RULE_LABELS = {
    ConsecutiveDifficulty: 'No consecutive difficulty levels',
    ConsecutiveAssignment: 'No back-to-back assignments',
    ConsecutiveHardTask: 'No consecutive hard tasks',
}

export function getRuleLabel(ruleKey) {
    return RULE_LABELS[ruleKey] ?? ruleKey
}