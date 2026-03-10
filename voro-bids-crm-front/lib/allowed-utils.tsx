export const rolesAllowed = ['Admin'];
export const routesAllowed = ['/', '/admin/forgot-password', '/admin/reset-password', '/admin/sign-in', '/admin/verify-code', '/admin/confirm-email'];

/**
 * Regex patterns for dynamic public routes (e.g. /admin/[slug]/sign-in).
 * Used alongside routesAllowed in the auth guard.
 */
export const routesAllowedPatterns = [
    /^\/admin\/[^/]+\/(sign-in|forgot-password|reset-password|verify-code|confirm-email)(\/|$)/,
]
