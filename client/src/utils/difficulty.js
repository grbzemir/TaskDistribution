export const DIFFICULTY_MIN = 1
export const DIFFICULTY_MAX = 8

export const DIFFICULTY_LEVELS = Array.from(
    { length: DIFFICULTY_MAX - DIFFICULTY_MIN + 1 },
    (_, index) => DIFFICULTY_MIN + index,
)

export function getDifficultyTier(level) {
    if (level == null) return 'none'
    if (level <= 3) return 'low'
    if (level <= 5) return 'medium'
    return 'high'
}